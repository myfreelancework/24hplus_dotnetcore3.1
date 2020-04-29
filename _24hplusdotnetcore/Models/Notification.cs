using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string userName { get; set; }
        public string type { get; set; }
        public string green { get; set; }
        public string recordId { get; set; }
        public string message { get; set; }
        public DateTime createAt { get; set; }
        public bool isRead { get; set; }
    }
}
