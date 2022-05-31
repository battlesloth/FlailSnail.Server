using Dapper;
using FlailSnail.Server.Configuration;
using FlailSnail.Server.Database.SQL;
using FlailSnail.Server.DTO;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FlailSnail.Server.Database
{
    public class NpgUserRepository : IUserRepository
    {
        private readonly IOptionsMonitor<DatabaseOptions> options;

        public NpgUserRepository(IOptionsMonitor<DatabaseOptions> databaseOptions)
        {
            options = databaseOptions;
        }

        public async Task<bool> UserExists(string email)
        {
            await using var conn = new NpgsqlConnection(options.CurrentValue.ConnString());

            return await conn.QueryFirstAsync<int>(UserSql.UserExists,
                new
                {
                    email
                }) > 0;
        }

        public async Task<int> InsertUser(User user)
        {
            await using var conn = new NpgsqlConnection(options.CurrentValue.ConnString());

            var result = await conn.QueryFirstAsync<int>(UserSql.InsertUser,
                new
                {
                    email = user.Email,
                    saltedHash = user.SaltedHash,
                    createdOn = DateTime.Now,
                    lastLogIn = DateTime.Now,
                    active = true,
                    disabledOn = DateTime.MinValue
                });

            return result;
        }

        public async Task<User> GetUser(string email)
        {
            await using var conn = new NpgsqlConnection(options.CurrentValue.ConnString());

            var result = await conn.QueryFirstOrDefaultAsync<User>(UserSql.SelectUserByEmail,
                new
                {
                    email
                });

            return result;
        }

        public async Task<bool> DeactivateUser(int userId)
        {
            await using var conn = new NpgsqlConnection(options.CurrentValue.ConnString());

            var result = await conn.ExecuteAsync(UserSql.DeactivateUser,
                new
                {
                    userId
                });

            return result > 0;
        }

        public async Task UserLoggedIn(int userId)
        {
            await using var conn = new NpgsqlConnection(options.CurrentValue.ConnString());

            var result = await conn.ExecuteAsync(UserSql.UserLoggedIn,
                new
                {
                    userId
                });
        }
    }
}
