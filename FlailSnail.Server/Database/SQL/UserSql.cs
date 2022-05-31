namespace FlailSnail.Server.Database.SQL
{
    public class UserSql
    {
        public const string UserExists = @"
SELECT COUNT(*)
FROM users
WHERE email = @email
";

        public const string SelectUserByEmail = @"
SELECT
    user_id,
    email,
    salted_hash,
    created_on,
    last_log_in,
    active,
    disabled_on
FROM users
WHERE email = @email;
";

        public const string SelectAllUsers = @"
SELECT
    user_id,
    email,
    salted_hash,
    created_on,
    last_log_in,
    active,
    disabled_on
FROM users;
";

        public const string InsertUser = @"
INSERT INTO users (email, salted_hash, created_on, last_log_in, active, disabled_on)
VALUES (@email, @saltedHash, @createdOn, @lastLogIn, @active, @disabledOn)
RETURNING user_id;
";

        public const string UpdateUser = @"
UPDATE users 
SET email=@email, 
    salted_hash = @saltedHash, 
    created_on = @createdOn, 
    last_log_in = @lastLogIn, 
    active = @active,
    disabled_on = @disabledOn
WHERE user_id = @userId;
";

        public const string DeactivateUser = @"
UPDATE users 
SET active = false,
    disabled_on = CURRENT_DATE
WHERE user_id = @userId;
";

        public const string UserLoggedIn = @"
UPDATE users 
SET last_log_in = CURRENT_DATE
WHERE user_id = @userId;
";
    }
}
