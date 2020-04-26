using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class AuthRefresh
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AuthRefreshId { get; set; }
        public string UserName { get; set; }
        public string RefresToken { get; set; }
    }
}
