using Authorization.Infrastructure.Interface.Mongo;

namespace Authorization.Infrastructure.Mongo
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}