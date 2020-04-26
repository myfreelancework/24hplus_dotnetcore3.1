using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace _24hplusdotnetcore.Models
{
    public class Demo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Desc { get; set; }

        public BsonDateTime CreateDate { get; set; }
    }
}