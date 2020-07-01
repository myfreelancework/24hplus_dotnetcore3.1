using System.Collections.Generic;

namespace _24hplusdotnetcore.ModelDtos
{
    public class CustomerCheckListResponseModel
    {
        public IEnumerable<GroupDtoModel> CheckList { get; set; }
    }

    public class GroupDtoModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public bool Mandatory { get; set; }
        public bool HasAlternate { get; set; }
        public IEnumerable<DocumentDtoModel> Documents { get; set; }
        // public IEnumerable<int> AlternateGroups { get; set; }
    }

    public class DocumentDtoModel
    {
        public int Id { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string InputDocUid { get; set; }
        public string MapBpmVar { get; set; }
    }
}
