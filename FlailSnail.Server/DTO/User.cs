namespace FlailSnail.Server.DTO
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string SaltedHash { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastLogIn { get; set; }
        public bool Active { get; set; }
        public DateTime DisabledOn { get; set; }
    }
}
