using _24hplusdotnetcore.Common.Attributes;
using System.Collections.Generic;

namespace _24hplusdotnetcore.ModelDtos
{
    public class GetCaseMCResponseDto
    {
        public int Id { get; set; }
        public string CreatedDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public string AppId { get; set; }
        public int? AppNumber { get; set; }
        public int? CreditAppId { get; set; }
        public string CustomerName { get; set; }
        public string CitizenId { get; set; }
        public string ProductName { get; set; }
        public decimal? LoanAmount { get; set; }
        public decimal? LoanTenor { get; set; }
        public string HasInsurrance { get; set; }
        public string TempResidence { get; set; }
        public string KioskAddress { get; set; }
        public string BpmStatus { get; set; }
        public object CLobChecklist { get; set; }
        [Newtonsoft.Json.JsonConverter(typeof(StringTypeConverter))]
        public CaseMCCheckListDto Checklist { get; set; }
        public IEnumerable<CaseMCReasonDto> Reasons { get; set; }
        public IEnumerable<CaseMCPdfFileDto> PdfFiles { get; set; }
    }

    public class CaseMCReasonDto
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string ReasonDetail { get; set; }
        public string UserComment { get; set; }
    }
    public class CaseMCPdfFileDto
    {
        public int Id { get; set; }
        public long CreatedDate { get; set; }
        public string RemotePathServer { get; set; }
        public string FileName { get; set; }
    }
    public class CaseMCCheckListDto
    {
        public IEnumerable<CaseMCCheckListItemDto> Checklist { get; set; }
    }
    public class CaseMCCheckListItemDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? Mandatory { get; set; }
        public bool? HasAlternate { get; set; }
        public IEnumerable<CaseMCDocumentDto> Documents { get; set; }
    }
    public class CaseMCDocumentDto
    {
        public int Id { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string InputDocUid { get; set; }
        public string MapBpmVar { get; set; }
    }
}
