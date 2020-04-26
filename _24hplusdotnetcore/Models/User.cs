using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        [BsonRequired]
        public string UserName { get; set; }

        [BsonRequired]
        public string UserFirstName { get; set; }

        [BsonRequired]
        public string UserMiddleName { get; set; }

        [BsonRequired]
        public string UserLastName { get; set; }

        [BsonRequired]
        public string UserEmail { get; set; }

        [BsonRequired]
        public string UserPassword { get; set; }

        [BsonRequired]
        public string RoleName { get; set; }
    }
}