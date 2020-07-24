using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace _24hplusdotnetcore.Models
{
    public class DataProcessing
    {
        public DataProcessing()
        {
            Status = Common.DataProcessingStatus.IN_PROGRESS;
            CreateDate = DateTime.UtcNow;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string LeadCrmId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string DataProcessingType { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? FinishDate { get; set; }
    }
}
