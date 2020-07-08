using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace _24hplusdotnetcore.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("productId")]
        public string ProductId { get; set; }
        [JsonProperty("productCategoryId")]
        public string ProductCategoryId { get; set; }
        [JsonProperty("productName")]
        public string ProductName { get; set; }
        [JsonProperty("customAge")]
        public string CustomAge { get; set; }
        [JsonProperty("duration")]
        public string Duration { get; set; }
        [JsonProperty("interestRateByMonth")]
        public string InterestRateByMonth { get; set; }
        [JsonProperty("interestRateByYear")]
        public string InterestRateByYear{ get; set; }
        [JsonProperty("documentRequired")]
        public string[] DocumentRequired { get; set; }
        [JsonProperty("otherDocument")]
        public string[] OtherDocument { get; set; }
        [JsonProperty("greenType")]
        public string GreenType { get; set; }
        [JsonProperty("productIdMC")]
        public int ProductIdMC { get; set; }
        [JsonProperty("productCodeMC")]
        public string ProductCodeMC { get; set; }
        [JsonProperty("productType")]
        public string ProductType { get; set; }
    }
}
