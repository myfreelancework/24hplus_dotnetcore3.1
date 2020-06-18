using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models.GCC
{
    public class GCCPersonalInsurance
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string agency_id { get; set; }
        public string program { get; set; }
        public string package { get; set; }
        public string request_code { get; set; }
        public string buy_fullname { get; set; }
        public string buy_bod { get; set; }
        public string buy_address { get; set; }
        public string buy_phone { get; set; }
        public string buy_cmnd { get; set; }
        public string buy_email { get; set; }
        public string buy_gender { get; set; }
        public string url_callback { get; set; }
        public Person[] person { get; set; }

        public DateTime CreateDate { get; set; }
        public string state { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public string link { get; set; }
    }

    public class Person
    {
        public string fullname { get; set; }
        public string cmnd { get; set; }
        public string bod { get; set; }
        public string relationship { get; set; }
    }

}
