using Newtonsoft.Json;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MCSendCaseNoteRequestDto
    {
        [JsonProperty("appNumber")]
        public int AppNumber { get; set; }
        [JsonProperty("noteContent")]
        public string NoteContent { get; set; }
    }
}
