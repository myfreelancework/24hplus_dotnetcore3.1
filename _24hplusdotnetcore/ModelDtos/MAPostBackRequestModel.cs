using _24hplusdotnetcore.Common.Constants;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MAPostBackRequestModel
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }
        [JsonProperty("date_of_lead")]
        public string DateOfLead { get; set; }
        [JsonProperty("dc_code")]
        public string DcCode { get; set; }
        [JsonProperty("dc_name")]
        public string DcName { get; set; }
        [JsonProperty("place_of_upload")]
        public string PlaceOfUpload { get; set; }
        [JsonProperty("document_collected")]
        public string DocumentCollected { get; set; }
        [JsonProperty("last_cast_status")]
        public string LastCastStatus { get; set; }
        [JsonProperty("dc_last_note")]
        public string DcLastNote { get; set; }
        [JsonProperty("app_schedule")]
        public string AppSchedule { get; set; }
        [JsonProperty("last_call")]
        public string LastCall { get; set; }
        [Required]
        [JsonProperty("lead_id")]
        public string LeadId { get; set; }
        [Required]
        public short Status { get; set; }
        [Required]
        [JsonProperty("detail_status")]
        public short DetailStatus { get; set; }

        public string GetStatusMessage()
        {
            string key = string.Format(StatusMapping.STATUS_PREFIX, Status);
            if (StatusMapping.MA_STATUS_MESSAGE_MAPPING.TryGetValue(key, out string message))
            {
                return message;
            }
            return StatusMapping.MA_STATUS_MESSAGE_MAPPING[StatusMapping.DEFAULT];
        }
        public string GetDetailStatusMessage()
        {
            string key = string.Format(StatusMapping.DETAIL_STATUS_PREFIX, Status, DetailStatus);
            if(StatusMapping.MA_DETAIL_STATUS_MESSAGE_MAPPING.TryGetValue(key, out string message))
            {
                return message;
            }
            return StatusMapping.MA_DETAIL_STATUS_MESSAGE_MAPPING[StatusMapping.DEFAULT];
        }
    }
}
