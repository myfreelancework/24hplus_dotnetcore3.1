using System;

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
        public bool Result { get; set; }
        public string Message { get; set; }
    }
}