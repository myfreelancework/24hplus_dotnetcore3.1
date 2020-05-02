using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models.GCC
{
    public class GCCPersonalProduct
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string agency_id { get; set; }
        public string program { get; set; }
        public string package { get; set; }
    }
}
