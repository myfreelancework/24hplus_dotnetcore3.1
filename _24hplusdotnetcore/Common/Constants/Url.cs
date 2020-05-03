namespace _24hplusdotnetcore.Common.Constants
{
    public static class Url
    {

        // GCC url
        public static string GCC_BASE_URL = "https://sandbox.globalcare.vn/";
        public static string GCC_GET_SSO_KEY = "gateway/v1/get-key.json?client_code={0}&client_secret={1}";
        public static string GCC_PUSH_DATA = "gateway/v1/order-create.json?client_secret={0}&key={1}";

        // MC
        public static string MC_BASE_URL = "https://uat-mfs-v2.mcredit.com.vn:8043/mcMobileService/service/v1.0/";
        public static string MC_LOGIN = "authorization";
        public static string MC_CHECK_INFO = "mobile-4sales/check-cic/check?citizenID={0}&customerName={1}";
        public static string MC_CHECK_DUPLICATE = "mobile-4sales/check-identifier?citizenId={0}";
    }
}