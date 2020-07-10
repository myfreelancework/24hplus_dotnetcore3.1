using Refit;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MCUpdateCICDto
    {
        [AliasAs("requestId")]
        public string RequestId { get; set; }
        [AliasAs("identifier")]
        public string Identifier { get; set; }
        [AliasAs("customerName")]
        public string CustomerName { get; set; }
        [AliasAs("cicResult")]
        public string CicResult { get; set; }
        [AliasAs("description")]
        public string Description { get; set; }
        [AliasAs("cicImageLink")]
        public string CicImageLink { get; set; }
        [AliasAs("lastUpdateTime")]
        public string LastUpdateTime { get; set; }
        [AliasAs("status")]
        public string Status { get; set; }
    }
}
