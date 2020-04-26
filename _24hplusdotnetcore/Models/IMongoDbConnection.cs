namespace _24hplusdotnetcore.Models
{
    public interface IMongoDbConnection
    {
        string ConnectionString {get;set;}
        string DataBase {get;set;}
    }
}