using System.ComponentModel.DataAnnotations;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MAPostBackRequestModel
    {
        [Required]
        public string Lead_id { get; set; }
        [Required]
        public short Status { get; set; }
        [Required]
        public short Detail_status { get; set; }
    }
}
