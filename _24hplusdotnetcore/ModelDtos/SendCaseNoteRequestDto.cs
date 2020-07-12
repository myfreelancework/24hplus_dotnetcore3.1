using System.ComponentModel.DataAnnotations;

namespace _24hplusdotnetcore.ModelDtos
{
    public class SendCaseNoteRequestDto
    {
        [Required]
        public string CustomerId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Trường Nội dung note bắt buộc phải nhập")]
        public string NoteContent { get; set; }
    }
}
