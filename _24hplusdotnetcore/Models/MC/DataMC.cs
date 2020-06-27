using Newtonsoft.Json;
using System.Collections.Generic;

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
        public string AppStatus { get; set; }

        [JsonProperty("md5")]
        public string Md5 { get; set; }

        [JsonProperty("info")]
        public List<Info> Info { get; set; }
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
        public string GroupId { get; set; }
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
        public string ProductId { get; set; }

        [JsonProperty("citizenId")]
        public string CitizenId { get; set; }

        [JsonProperty("tempResidence")]
        public string TempResidence { get; set; }

        [JsonProperty("loanAmount")]
        public string LoanAmount { get; set; }

        [JsonProperty("loanTenor")]
        public string LoanTenor { get; set; }

        [JsonProperty("hasInsurance")]
        public string HasInsurance { get; set; }

        [JsonProperty("issuePlace")]
        public string IssuePlace { get; set; }

        [JsonProperty("shopCode")]
        public string ShopCode { get; set; }

        [JsonProperty("companyTaxNumber")]
        public string CompanyTaxNumber { get; set; }
    }
}
