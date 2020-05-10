using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models.GCC
{
    public class GCCMotoProgramModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public double ty_le { get; set; }
        public int fee_min { get; set; }
        public int fee_min_with_zutturide { get; set; }
    }
}
