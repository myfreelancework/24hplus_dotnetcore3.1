using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace _24hplusdotnetcore.Models
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ContractCode { get; set; }
        [BsonRequired]
        public string UserName { get; set; }
        [BsonRequired]
        public string Status { get; set; }
        public string GreenType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public Personal Personal { get; set; }
        public Address ResidentAddress { get; set; }
        public Address TemporaryAddress { get; set; }

        public Working Working { get; set; }
        public Loan Loan { get; set; }
        public Family Family { get; set; }
        public ReferencePerson ReferencePerson1 { get; set; }
        public ReferencePerson ReferencePerson2 { get; set; }


        public Vender Vender1 { get; set; }
        public Vender Vender2 { get; set; }

        // public HopDongBHNT HopDongBHNT { get; set; }

        public Disburement Disburement { get; set; }
        public Sale SaleInfo { get; set; }
        public Return Return { get; set; }
    }

    public class Personal {
        public string Name { get; set; }
        public string IdCard { get; set; }
        public string IdCardProvince { get; set; }
        public string IdCardDate { get; set; }
        public string DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string EducationLevel { get; set; }
        public string MaritalStatus { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
    }

    public class Address {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }
    }

    public class Living {
        public string StayedYear { get; set; }
        public string StayedMonth { get; set; }
        public string HouseType { get; set; }
    }

    public class Working {
        public string Job { get; set; }
        public string Position { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string TaxId { get; set; }
        public string Income { get; set; }
        public string IncomeMethod { get; set; }
        public string LaborType { get; set; }
        public string Spend { get; set; }

        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public Address CompanyAddress { get; set; }

        public string BranchName { get; set; }
        public string BranchPhone { get; set; }
        public Address BranchAddress { get; set; }
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
        public string Product { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public string BuyInsurance { get; set; }
    }

    public class Disburement
    {
        public string SignAddress { get; set; }
        public string Method { get; set; }
        public string Name { get; set; }
        public string BankName { get; set; }
        public string BankBranch { get; set; }
        public string BankAccount { get; set; }
    }

    public class Vender
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class ReferencePerson
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Relationship { get; set; }
    }

    public class Family
    {
        public string Relationship { get; set; }
        public string OtherRelationship { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string IdCard { get; set; }
        public string Phone { get; set; }
        public string Dependency { get; set; }
        public string Address { get; set; }
    }
    public class Return 
    {
        public string Department { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
    }
}
