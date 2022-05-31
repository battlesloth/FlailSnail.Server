namespace FlailSnail.Server.Configuration
{
    public class DatabaseOptions
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public string ConnString()
        {
            return $"Server={Host};Port={Port};Database={Database};Userid={User};Password={Password};";
        }
    }
}
