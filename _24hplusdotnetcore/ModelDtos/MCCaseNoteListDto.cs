using System.Collections.Generic;
using Newtonsoft.Json;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MCCaseNoteListDto
    {

        [JsonProperty("app_notes_entries")]
        public MCNotesEntriesModel MCNotesEntries { get; set; }
    }

    public class MCNotesEntriesModel
    {

        [JsonProperty("app_notes_entry")]
        public IEnumerable<MCNotesEntrieModel> MCNotesEntry { get; set; }
    }

    public class MCNotesEntrieModel
    {

        [JsonProperty("idapp_uid")]
        public string IdappUid { get; set; }
        [JsonProperty("note_content")]
        public string NoteContent { get; set; }
        [JsonProperty("note_date")]
        public string NoteDate { get; set; }
        [JsonProperty("usr_uid")]
        public string UsrUid { get; set; }
    }
}
