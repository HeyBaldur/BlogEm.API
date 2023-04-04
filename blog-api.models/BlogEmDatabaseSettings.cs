namespace blog_api.models
{
    public class BlogEmDatabaseSettings : IUserDatabaseSettings
    {
        public string UserCollection { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IUserDatabaseSettings
    {
        string UserCollection { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
