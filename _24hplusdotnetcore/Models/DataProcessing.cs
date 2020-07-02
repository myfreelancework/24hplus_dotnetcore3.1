using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class DataProcessing
    {
        public DataProcessing()
        {
            Status = Common.DataProcessingStatus.IN_PROGRESS;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string Status { get; set; }
        public string DataProcessingType { get; set; }
    }
}
