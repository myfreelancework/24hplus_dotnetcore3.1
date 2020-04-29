using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Models
{
    public class PaymentCalc
    {
        public int amountReceive { get; set; }
        public int amountInsurance { get; set; }
        public int amountLoan { get; set; }
        public string productCategory { get; set; }
        public string product { get; set; }
        public int term { get; set; }
        public string dateReceive { get; set; }

    }
}
