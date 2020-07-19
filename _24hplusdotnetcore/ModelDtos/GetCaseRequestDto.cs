using _24hplusdotnetcore.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace _24hplusdotnetcore.ModelDtos
{
    public class GetCaseRequestDto
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;
        public string Keyword { get; set; } = string.Empty;
        [EnumDataType(typeof(CaseStatus))]
        public CaseStatus Status { get; set; } = CaseStatus.PROCESSING;
        public string SaleCode { get; set; } = string.Empty;
    }
}
