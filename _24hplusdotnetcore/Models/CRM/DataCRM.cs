using Newtonsoft.Json;
using System;

namespace _24hplusdotnetcore.Models.CRM
{
    public class DataCRM
    {
        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("idNo")]
        public string IdNo { get; set; }

        [JsonProperty("income")]
        public string Income { get; set; }

        [JsonProperty("appointmentDate")]
        public DateTimeOffset AppointmentDate { get; set; }

        [JsonProperty("appointmentAdd")]
        public string AppointmentAdd { get; set; }

        [JsonProperty("finaceCom")]
        public string FinaceCom { get; set; }

        [JsonProperty("prod")]
        public string Prod { get; set; }

        [JsonProperty("subProd")]
        public string SubProd { get; set; }

        [JsonProperty("loanAmount")]
        public string LoanAmount { get; set; }

        [JsonProperty("tenor")]
        public string Tenor { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("requestDoc")]
        public string RequestDoc { get; set; }

        [JsonProperty("job")]
        public string Job { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("leadsource")]
        public string Leadsource { get; set; }

        [JsonProperty("yearold")]
        public string Yearold { get; set; }

        [JsonProperty("requestDoc_cs")]
        public string RequestDocCs { get; set; }

        [JsonProperty("Noted_cs")]
        public string NotedCs { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }
    }
}
