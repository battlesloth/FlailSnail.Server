namespace FlailSnail.Server.Security
{
    public class JwtPayload
    {
        public string UserId;
        public string Email;

        public JwtPayload(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
