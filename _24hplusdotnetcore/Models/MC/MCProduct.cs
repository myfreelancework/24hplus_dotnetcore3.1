using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Models.MC
{
    public class MCProduct
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("ccy")]
        public string Ccy { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdDate")]
        public string CreatedDate { get; set; }

        [JsonProperty("endEffDate")]
        public string EndEffDate { get; set; }

        [JsonProperty("lastUpdatedBy")]
        public string LastUpdatedBy { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public string LastUpdatedDate { get; set; }

        [JsonProperty("latePenaltyFee")]
        public string LatePenaltyFee { get; set; }

        [JsonProperty("lateRateIndex")]
        public string LateRateIndex { get; set; }

        [JsonProperty("maxLoanAmount")]
        public string MaxLoanAmount { get; set; }

        [JsonProperty("maxQuantityCommodities")]
        public string MaxQuantityCommodities { get; set; }

        [JsonProperty("maxTenor")]
        public string MaxTenor { get; set; }

        [JsonProperty("minLoanAmount")]
        public string MinLoanAmount { get; set; }

        [JsonProperty("minTenor")]
        public string MinTenor { get; set; }

        [JsonProperty("preLiquidationFee")]
        public string PreLiquidationFee { get; set; }

        [JsonProperty("productCategoryId")]
        public string ProductCategoryId { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("productGroupId")]
        public string ProductGroupId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("pti")]
        public string Pti { get; set; }

        [JsonProperty("rateIndex")]
        public string RateIndex { get; set; }

        [JsonProperty("recordStatus")]
        public string RecordStatus { get; set; }

        [JsonProperty("startEffDate")]
        public string StartEffDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("tenor")]
        public string Tenor { get; set; }

        [JsonProperty("isCheckCat")]
        public string IsCheckCat { get; set; }

        [JsonProperty("productGroupName")]
        public string ProductGroupName { get; set; }
    }
}
