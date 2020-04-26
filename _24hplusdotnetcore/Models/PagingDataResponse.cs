namespace _24hplusdotnetcore.Models
{
    public class PagingDataResponse
    {
        public int code { get; set; }
        public string  message { get; set; }
        public dynamic data { get; set; }
        public int pagenumber { get; set; }
        public long totalpage { get; set; }
        public long totalrecord { get; set; }
    }
}