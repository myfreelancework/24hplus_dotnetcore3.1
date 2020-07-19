using Refit;

namespace _24hplusdotnetcore.ModelDtos
{
    public class GetCaseMCRequestDto
    {
        [AliasAs("pageNumber")]
        public int PageNumber { get; set; }
        [AliasAs("pageSize")]
        public int PageSize { get; set; } 
        [AliasAs("keyword")]
        public string Keyword { get; set; }
        [AliasAs("status")]
        public string Status { get; set; }
        [AliasAs("saleCode")]
        public string SaleCode { get; set; }
    }
}
