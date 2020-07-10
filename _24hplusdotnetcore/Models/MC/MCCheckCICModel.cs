
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Refit;

namespace _24hplusdotnetcore.Models.MC
{
    public class MCCheckCICModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [AliasAs("requestId")]
        public string RequestId { get; set; }
        [AliasAs("identifier")]
        public string Identifier { get; set; }
        [AliasAs("customerName")]
        public string CustomerName { get; set; }
        [AliasAs("cicResult")]
        public string CicResult { get; set; }
        [AliasAs("description")]
        public string Description { get; set; }
        [AliasAs("cicImageLink")]
        public string CicImageLink { get; set; }
        [AliasAs("lastUpdateTime")]
        public string LastUpdateTime { get; set; }
        [AliasAs("status")]
        public string Status { get; set; }

        [AliasAs("createDate")]
        public DateTime CreateDate { get; set; }
    }
}