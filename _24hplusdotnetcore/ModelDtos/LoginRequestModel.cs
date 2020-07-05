using Newtonsoft.Json;

namespace _24hplusdotnetcore.ModelDtos
{
    public class LoginRequestModel
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("notificationId")]
        public string NotificationId { get; set; }
        [JsonProperty("imei")]
        public string Imei { get; set; }
        [JsonProperty("osType")]
        public string OsType { get; set; }
    }
}
