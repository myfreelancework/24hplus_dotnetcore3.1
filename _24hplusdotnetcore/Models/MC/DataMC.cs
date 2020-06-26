using Newtonsoft.Json;

namespace _24hplusdotnetcore.Models.MC
{
    public partial class DataMC
    {
        [JsonProperty("request")]
        public Request Request { get; set; }

        [JsonProperty("mobileProductType")]
        public string MobileProductType { get; set; }

        [JsonProperty("mobileIssueDateCitizen")]
        public string MobileIssueDateCitizen { get; set; }

        [JsonProperty("appStatus")]
        public long AppStatus { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("info")]
        public Info[] Info { get; set; }
    }

    public partial class Info
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("documentCode")]
        public string DocumentCode { get; set; }

        [JsonProperty("mimeType")]
        public string MimeType { get; set; }

        [JsonProperty("groupId")]
        public long GroupId { get; set; }
    }

    public partial class Request
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("SaleCode")]
        public string SaleCode { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("productId")]
        public long ProductId { get; set; }

        [JsonProperty("citizenId")]
        public string CitizenId { get; set; }

        [JsonProperty("tempResidence")]
        public long TempResidence { get; set; }

        [JsonProperty("loanAmount")]
        public long LoanAmount { get; set; }

        [JsonProperty("loanTenor")]
        public long LoanTenor { get; set; }

        [JsonProperty("hasInsurance")]
        public long HasInsurance { get; set; }

        [JsonProperty("issuePlace")]
        public string IssuePlace { get; set; }

        [JsonProperty("shopCode")]
        public string ShopCode { get; set; }

        [JsonProperty("companyTaxNumber")]
        public long CompanyTaxNumber { get; set; }
    }
}
