using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlailSnail.Server.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FlailSnail.Server.Security
{

    public class JwtTokenService : ITokenService
    {
        private readonly ILogger<JwtTokenService> logger;

        private readonly int expiryDuration = 30;
        private readonly IOptionsMonitor<ServiceOptions> options;

        public JwtTokenService(ILogger<JwtTokenService> logger, IOptionsMonitor<ServiceOptions> options)
        {
            this.logger = logger;
            this.options = options;
        }

        public string GetToken(JwtPayload payload)
        {
            if (payload == null)
            {
                throw new ArgumentException(nameof(payload));
            }

            var claims = new[]
            {
                new Claim("userId", payload.UserId),
                new Claim("email", payload.Email)
            };

            return GenerateToken(claims);
        }

        public bool ValidateToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.CurrentValue.JwtSecret));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = options.CurrentValue.JwtIssuer,
                        ValidAudience = options.CurrentValue.JwtIssuer,
                        IssuerSigningKey = securityKey
                    }, out SecurityToken validatedToken);
                
                return validatedToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public string ReissueToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!ValidateToken(token))
            {
                return string.Empty;
            }
            
            var jwt = tokenHandler.ReadJwtToken(token);

            return GenerateToken(jwt.Claims);
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.CurrentValue.JwtSecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            
            var tokenDescriptor = new JwtSecurityToken(
                options.CurrentValue.JwtIssuer, 
                options.CurrentValue.JwtIssuer,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiryDuration), 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
