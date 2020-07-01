using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ContractCode { get; set; } // @todo
        [BsonRequired]
        public string UserName { get; set; }
        [BsonRequired]
        public string Status { get; set; }
        public string GreenType { get; set; } // @todo
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Personal Personal { get; set; } // 80%
        public Address ResidentAddress { get; set; } // @todo
        public Boolean IsTheSameResidentAddress { get; set; }
        public Address TemporaryAddress { get; set; } // @todo
        public Working Working { get; set; }
        public Loan Loan { get; set; }
        public Sale SaleInfo { get; set; }
        public Result Result { get; set; }
        public IEnumerable<GroupDocument> Documents  { get; set; }
    }

    public class Personal
    {
        public string Name { get; set; } // potentialname
        public string IdCard { get; set; } // cf_1050
        public string IdCardProvince { get; set; } // 
        public string IdCardDate { get; set; } // cf_1350
        public string DateOfBirth { get; set; } // cf_948
        public string Phone { get; set; } // cf_854
        public string SubPhone { get; set; } // cf_854
        public string EducationLevel { get; set; } // @todo
        public string MaritalStatus { get; set; } //cf_1030
        public string Gender { get; set; } // cf_1026
        public string Email { get; set; } // cf_1028
    }

    public class Address
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }
    }
    public class Working
    {
        public string Job { get; set; }
        public string Position { get; set; }
        public string TaxId { get; set; }
        public string Income { get; set; }
        public string IncomeMethod { get; set; }
        public string LaborType { get; set; }
        public string OtherLoans { get; set; }

        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public Address CompanyAddress { get; set; }
    }

    public class Sale
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }
    }

    public class Loan
    {
        public string Purpose { get; set; }
        public string Term { get; set; }
        public string Category { get; set; }
        public string CategoryId { get; set; }
        public string Product { get; set; }
        public string ProductId { get; set; }
        public string Amount { get; set; }
        public string BuyInsurance { get; set; }
        public string SignAddress { get; set; }
    }

    public class Result
    {
        public string Department { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
    }

    public class GroupDocument
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public bool Mandatory { get; set; }
        public bool HasAlternate { get; set; }
        public IEnumerable<DocumentUpload> Documents { get; set; }
        public IEnumerable<int> AlternateGroups { get; set; }
    }

    public class DocumentUpload
    {
        public int Id { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public string InputDocUid { get; set; }
        public string MapBpmVar { get; set; }
        public IEnumerable<UploadedMedia> UploadedMedias { get; set; }
    }
    public class UploadedMedia {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Uri { get; set; }

    }
}
