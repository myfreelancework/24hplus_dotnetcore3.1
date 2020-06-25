using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace _24hplusdotnetcore.Models
{
    public class FileUpload
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        [JsonProperty("fileUploadId")]
        public string FileUploadId { get; set; }
        [JsonProperty("documentCategoryId")]
        public string DocumentCategoryId { get; set; }
        [JsonProperty("fileUploadName")]
        public string FileUploadName { get; set; }
        [JsonProperty("fileUploadURL")]
        public string FileUploadURL { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
        [JsonProperty("groupId")]
        public string GroupId { get; set; }
        [JsonProperty("documentCode")]
        public string DocumentCode { get; set; }
        [JsonProperty("documentName")]
        public string DocumentName { get; set; }
        [JsonProperty("inputDocUid")]
        public string InputDocUid { get; set; }
        [JsonProperty("mapBpmVar")]
        public string MapBpmVar { get; set; }
    }
}
