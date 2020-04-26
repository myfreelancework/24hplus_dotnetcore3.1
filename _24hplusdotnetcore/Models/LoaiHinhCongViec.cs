using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class JobCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRequired]
        public string JobCategoryId { get; set; }
        public string JobCategoryName { get; set; }
        public string GreenType { get; set; }
    }
}