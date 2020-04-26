namespace _24hplusdotnetcore.Models
{
    public class MongoDbConnection : IMongoDbConnection
    {
        public string ConnectionString { get; set; }
        public string DataBase { get; set; }
    }
}