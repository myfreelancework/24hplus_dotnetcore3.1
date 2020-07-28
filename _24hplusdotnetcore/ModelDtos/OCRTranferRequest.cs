using _24hplusdotnetcore.Common.Enums;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _24hplusdotnetcore.ModelDtos
{
    public class OCRTranferRequest
    {
        [Required]
        public OCRType Type { get; set; }
        [Required]
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
