using Refit;

namespace _24hplusdotnetcore.ModelDtos
{
    public class CustomerUpdateStatusDto
    {
        [AliasAs("customerId")]
        public string CustomerId { get; set; }
        [AliasAs("status")]
        public string Status { get; set; }
        [AliasAs("reason")]
        public string Reason { get; set; }
        [AliasAs("returnStatus")]
        public string ReturnStatus { get; set; }
        [AliasAs("leadsource")]
        public string LeadSource { get; set; }
    }
}
