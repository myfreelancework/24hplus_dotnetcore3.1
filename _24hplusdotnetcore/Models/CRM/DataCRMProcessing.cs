using _24hplusdotnetcore.Common;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace _24hplusdotnetcore.Models.CRM
{
    public class DataCRMProcessing
    {
        public DataCRMProcessing()
        {
            Status = DataCRMProcessingStatus.InProgress;
            CreateDate = DateTime.UtcNow;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string LeadCrmId { get; set; }
        public string Status { get; set; }
        public string LeadSource { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? FinishDate { get; set; }
    }
}
