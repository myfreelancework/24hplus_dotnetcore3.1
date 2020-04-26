using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class DocumentCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRequired]
        public string DocumentCategoryId { get; set; }
        public string DocumentCategoryName { get; set; }
        public string GreenType { get; set; }
    }
}