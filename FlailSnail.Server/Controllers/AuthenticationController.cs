using FlailSnail.Server.Database;
using Microsoft.AspNetCore.Mvc;

namespace FlailSnail.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> logger;
        private readonly IDatabaseConnector database;
        
        public AuthenticationController(ILogger<AuthenticationController> logger, IDatabaseConnector database)
        {
            this.logger = logger;
            this.database = database;
        }

        [HttpGet(Name = "login")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}