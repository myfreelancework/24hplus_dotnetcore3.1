using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string name { get; set; }
        public string idCard { get; set; }
        public string phone { get; set; }
        public string contactNumber { get; set; }
        public string disbursementDate { get; set; }
        public string expriedDate { get; set; }
        public string amountLoan { get; set; }
        public string amountPayment { get; set; }

    }
}
