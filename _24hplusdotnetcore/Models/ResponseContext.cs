using System;
using Newtonsoft.Json;

namespace _24hplusdotnetcore.Models
{
    [Serializable]
    public class ResponseContext
    {
        public int code { get; set; }
        public string message { get; set; }
        public dynamic data { get; set; }
    }

    public class ResponseMAContext
    {
        [JsonProperty("Result")]
        public bool Result { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}