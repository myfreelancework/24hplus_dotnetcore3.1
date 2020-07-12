using System.Collections.Generic;

namespace _24hplusdotnetcore.ModelDtos
{
    public class MCCaseNoteListDto
    {
        public IEnumerable<MCCaseNoteModel> CheckList { get; set; }
    }

    public class MCCaseNoteModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? Mandatory { get; set; }
        public int? HasAlternate { get; set; }
        public IEnumerable<MCCaseNoteDocument> Documents { get; set; }
    }

    public class MCCaseNoteDocument
    {
        public int Id { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string InputDocUid { get; set; }
        public string MapBpmVar { get; set; }
    }
}
