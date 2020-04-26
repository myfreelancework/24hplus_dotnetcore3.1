using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class FileUpload
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string FileUploadId { get; set; }
        public string DocumentCategoryId { get; set; }
        public string FileUploadName { get; set; }
        public string FileUploadURL { get; set; }
        public string CustomerId { get; set; }
    }
}
