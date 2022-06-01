using Microsoft.Extensions.Primitives;

namespace FlailSnail.Server.Security
{
    public interface ITokenService
    {
        string GetToken(JwtPayload payload);
        bool ValidateToken(string token);
        string ReissueToken(string token);
    }
}
