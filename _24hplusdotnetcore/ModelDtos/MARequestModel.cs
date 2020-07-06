using System;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MARequestModel
    {
        public MARequestModel()
        {
            TimeStamp = DateTime.UtcNow;
            Tag = Guid.NewGuid().ToString();
        }

        public string PublicKey { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Tag { get; set; }
        public MARequestDataModel Data { get; set; }
    }

    public class MARequestDataModel
    {
        public string LEAD_ID { get; set; }
        public string ID_NO { get; set; }
        public string CONTACT_NAME { get; set; }
        public string PHONE { get; set; }
        public string CURRENT_ADDRESS { get; set; }
        public string LOCATION { get; set; }
        public string PRODUCT { get; set; }
        public DateTime? T_STATUS_DATE { get; set; }
        public string APPOINTMENT_DATE { get; set; }
        public string APPOINTMENT_ADDRESS { get; set; }
        public string TSA_IN_CHARGE { get; set; }
        public string TST_TEAM { get; set; }
        public string REQUEST_DOCUMENT { get; set; }
        public string DOB { get; set; }
        public string GENDER { get; set; }
        public string COMPANY_NAME { get; set; }
        public string COMPANY_ADDR { get; set; }
        public string TEL_COMPANY { get; set; }
        public string AREA { get; set; }
        public string MARITAL_STATUS { get; set; }
        public string OWNER { get; set; }
        public string FAX_NO { get; set; }
        public string INCOME { get; set; }
        public string POSITION { get; set; }
        public string WORK_PERIOD { get; set; }
        public string TYPE_OF_CONTRACT { get; set; }
        public string HEALTH_CARD { get; set; }
        public string LOAN_AMOUNT { get; set; }
        public string TENURE { get; set; }
        public DateTime? APP_DATE { get; set; }
        public DateTime? DISBURSAL_DATE { get; set; }
        public DateTime? GENERATE_TO_LEAD { get; set; }
        public DateTime? FOLLOWED_DATE { get; set; }
        public string PERMANENT_ADDR { get; set; }
        public string TSA_NAME { get; set; }
        public string TSA_CAMPAIN { get; set; }
        public string TSA_GROUP { get; set; }
        public string TSA_LAST_NOTES { get; set; }
        public string OCCUPATION { get; set; }
        public string REQUEST_ID { get; set; }
        public string ROUTE { get; set; }
    }
}
