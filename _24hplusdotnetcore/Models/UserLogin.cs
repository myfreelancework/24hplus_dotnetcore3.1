using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class UserLogin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string LoginId { get; set; }
        [BsonRequired]
        public string UserName { get; set; }
        public string uuid { get; set; }
        public string ostype { get; set; }
        public string token { get; set; }
        public string registration_token { get; set; }

    }
}