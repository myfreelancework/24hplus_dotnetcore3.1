using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models.GCC
{
    public class GCCMotoInsuranceModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string product_code { get; set; }
        public string product_name { get; set; }
        public string agency_id { get; set; }
        public string program { get; set; }
        public string programTitle { get; set; }
        public GCCPackageMotoModel package { get; set; }
        public string request_code { get; set; }
        public string buy_fullname { get; set; }
        public string buy_bod { get; set; }
        public string buy_address { get; set; }
        public string buy_phone { get; set; }
        public string buy_cmnd { get; set; }
        public string buy_email { get; set; }
        public string buy_gender { get; set; }
        public string url_callback { get; set; }
        public GCCMotoBikeModel data { get; set; }
        public string insuraceFee { get; set; }

        public DateTime CreateDate { get; set; }
        public string state { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public string link { get; set; }
    }

    public class GCCMotoBikeModel
    {
        public Info info { get; set; }
        public string[] images { get; set; }
    }
    public class Info
    {
        public string price { get; set; }
        public string bsx { get; set; }
        public string sokhung { get; set; }
        public string somay { get; set; }
    }
    public class GCCPackageMotoModel
    {
        public string motoName { get; set; }
        public string motorcycle_id { get; set; }
        public string year { get; set; }
    }
}
