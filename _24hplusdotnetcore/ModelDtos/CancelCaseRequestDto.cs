using System;
using System.ComponentModel.DataAnnotations;

namespace _24hplusdotnetcore.ModelDtos
{
    public class CancelCaseRequestDto
    {
        [Required]
        public string CustomerId { get; set; }
        [Required(ErrorMessage = "Trường Lí do bắt buộc phải nhập")]
        [Range(0, 1, ErrorMessage = "Lí do không hợp lệ")]
        public int Reason { get; set; }
        // [Required(AllowEmptyStrings = false, ErrorMessage = "Trường Nhận xét bắt buộc phải nhập")]
        public string Comment { get; set; }
    }
}
