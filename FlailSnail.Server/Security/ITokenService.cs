namespace FlailSnail.Server.Security
{
    public interface ITokenService
    {
        string GetToken(string key, string issuer, JwtPayload payload);
        (bool success, string token) ValidateToken(string key, string issuer, string token);
    }
}
