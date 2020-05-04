using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace _24hplusdotnetcore.Models
{
    public class ConfigModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Key { get; set; }
        public dynamic Value { get; set; }
    }
}