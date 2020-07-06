
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Refit;

namespace _24hplusdotnetcore.Models.MC
{
    public class MCNotificationModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [AliasAs("mcId")]
        public int MCId { get; set; }
        [AliasAs("currentStatus")]
        public string CurrentStatus { get; set; }
        [AliasAs("appNumber")]
        public string AppNumber { get; set; }
        [AliasAs("appId")]
        public string AppId { get; set; }
    }
}