using Refit;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MCNotificationDto
    {
        [AliasAs("id")]
        public int Id { get; set; }
        [AliasAs("currentStatus")]
        public string CurrentStatus { get; set; }
        [AliasAs("appNumber")]
        public string AppNumber { get; set; }
        [AliasAs("appId")]
        public string AppId { get; set; }
    }
}
