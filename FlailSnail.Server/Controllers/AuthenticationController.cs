using FlailSnail.Server.Database;
using FlailSnail.Server.DTO;
using FlailSnail.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlailSnail.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> logger;
        private readonly IUserRepository userRepo;
        private readonly ITokenService tokenService;
        
        public AuthenticationController(ILogger<AuthenticationController> logger, IUserRepository database, ITokenService tokenService)
        {
            this.logger = logger;
            userRepo = database;
        }

        [AllowAnonymous]
        [HttpPost(Name = "login")]
        public async Task<IActionResult> Login([FromBody] Credentials creds)
        {
            var user = await userRepo.GetUser(creds.Email);

            if (user == null)
            {
                return NotFound();
            }

            if (!user.Active)
            {
                return new JsonResult(new { Result = "User disabled" });
            }

            if (!BCrypt.Net.BCrypt.Verify(creds.Password, user.SaltedHash))
            {
                return NotFound();
            }

            await userRepo.UserLoggedIn(user.UserId);

            var token = tokenService.GetToken(
                new JwtPayload(user.UserId.ToString(), user.Email));
            
            Response.Headers.Add("Set-Authorization", token);
            
            return new JsonResult(new {lastLogin = user.LastLogIn, token});

        }

        [AllowAnonymous]
        [HttpPost(Name = "register")]
        public async Task<IActionResult> Register([FromBody] Credentials creds)
        {
            if (await userRepo.UserExists(creds.Email))
            {
                return new JsonResult(new { Result = "User exists" });
            }

            var user = new User
            {
                Email = creds.Email,
                SaltedHash = BCrypt.Net.BCrypt.HashPassword(creds.Password)
            };

            var id = await userRepo.InsertUser(user);

            return new JsonResult(id);
        }
    }
}