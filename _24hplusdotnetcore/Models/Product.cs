using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string CustomAge { get; set; }
        public string Duration { get; set; }
        public string InterestRateByMonth { get; set; }
        public string InterestRateByYear{ get; set; }
        public string[] DocumentRequired { get; set; }
        public string[] OtherDocument { get; set; }
        public string GreenType { get; set; }
    }
}
