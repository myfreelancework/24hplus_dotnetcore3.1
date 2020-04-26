using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class Roles
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRequired]
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
    }
}
