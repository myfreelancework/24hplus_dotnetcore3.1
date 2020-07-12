using Newtonsoft.Json;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MCCancelCaseRequestDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("reason")]
        public int Reason { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
    }
}
