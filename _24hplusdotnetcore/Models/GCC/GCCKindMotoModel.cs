using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models.GCC
{
    public class GCCKindMotoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int motoId { get; set; }
        public string motoName { get; set; }
        public int price { get; set; }
    }
}
