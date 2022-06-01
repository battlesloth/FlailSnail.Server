using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FlailSnail.Server.Security
{

    public class JwtTokenService : ITokenService
    {
        private readonly int expiryDuration = 30;

        public string GetToken(string key, string issuer, JwtPayload payload)
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

            return GenerateToken(key, issuer, claims);
        }

        public (bool success, string token) ValidateToken(string key, string issuer, string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey = securityKey
                    }, out SecurityToken validatedToken);

                return (true, GenerateToken(key, issuer, principal.Claims.ToArray()));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (false, string.Empty);
            }
        }

        private string GenerateToken(string key, string issuer, Claim[] claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.AddMinutes(expiryDuration), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
