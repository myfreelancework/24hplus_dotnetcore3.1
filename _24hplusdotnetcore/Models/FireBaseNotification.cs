using Newtonsoft.Json;

namespace _24hplusdotnetcore.Models
{
    public class FireBaseNotification
    {
        [JsonProperty("notification")]
        public NotificationFireBase Notification { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("collapseKey")]
        public string CollapseKey { get; set; }

        [JsonProperty("registration_ids")]
        public string[] RegistrationIds { get; set; }
    }

    public class Data
    {
        [JsonProperty("notification_id")]
        public long NotificationId { get; set; }

        [JsonProperty("notification_type")]
        public string NotificationType { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("green_type")]
        public string GreenType { get; set; }

        [JsonProperty("record_id")]
        public string RecordId { get; set; }

        [JsonProperty("total_notifications")]
        public long TotalNotifications { get; set; }
    }

    public class NotificationFireBase
    {
        [JsonProperty("android")]
        public Android Android { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }

    public class Android
    {
    }
}
