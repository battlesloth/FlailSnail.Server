using FlailSnail.Server.DTO;

namespace FlailSnail.Server.Database
{
    public interface IUserRepository
    {
        Task<bool> UserExists(string email);

        Task<int> InsertUser(User user);

        Task<User> GetUser(string email);

        Task<bool> DeactivateUser(int userId);

        Task UserLoggedIn(int userId);
    }
}
