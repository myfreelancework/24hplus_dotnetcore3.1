using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models.MC
{
    public class DataMCProcessing
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? FinishDate { get; set; }
    }
}
