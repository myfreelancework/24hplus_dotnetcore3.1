namespace _24hplusdotnetcore.Common
{
    public static class Config
    {
        public static int PageSize = 20;
        public static string CredMC_URL = "https://uat-mfs-v2.mcredit.com.vn:8043/mcMobileService/service/v1.0/authorization";
        public static string CredMC_Security_Key = "MEKONG-CREDIT-57d733a9-bcb5-4bff-aca1-f58163122fae";
        public static string CredMC_Username = "mekongcredit.3rd";
        public static string CredMC_Password = "MK$2DG@4";
        public static string MC_CheckInfo_URL = "https://uat-mfs-v2.mcredit.com.vn:8043/mcMobileService/service/v1.0/mobile-4sales/check-cic/check?citizenID={0}&customerName={1}";
        public static string MC_CheckDuplicate_URL = "https://uat-mfs-v2.mcredit.com.vn:8043/mcMobileService/service/v1.0/mobile-4sales/check-identifier?citizenId={0}";
    }
}