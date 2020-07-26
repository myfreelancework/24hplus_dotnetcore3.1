using Refit;

namespace _24hplusdotnetcore.ModelDtos
{
    public class CustomerCheckListRequestModel
    {
        [AliasAs("mobileSchemaProductCode")]
        public string MobileSchemaProductCode { get; set; }
        [AliasAs("mobileTemResidence")]
        public int? MobileTemResidence { get; set; }
        [AliasAs("loanAmountAfterInsurrance")]
        public decimal? LoanAmountAfterInsurrance { get; set; }
        [AliasAs("shopCode")]
        public string ShopCode { get; set; }
        [AliasAs("customerName")]
        public string CustomerName { get; set; }
        [AliasAs("citizenId")]
        public string CitizenId { get; set; }
        [AliasAs("loanTenor")]
        public string LoanTenor { get; set; }
        [AliasAs("hasInsurance")]
        public bool HasInsurance { get; set; }
        [AliasAs("companyTaxNumber")]
        public string CompanyTaxNumber { get; set; }
        [AliasAs("hasCourier")]
        public int HasCourier { get; set; }
    }
}
