using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class ProductCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ProductCategoryId { get; set; }
        [BsonRequired]
        public string ProductCategoryName { get; set; }
        public string GreenType { get; set; }
        public string Note { get; set; }
    }
}
