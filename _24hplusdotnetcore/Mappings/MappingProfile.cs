using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.CRM;
using _24hplusdotnetcore.Models.MC;
using AutoMapper;

namespace _24hplusdotnetcore.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region CRM to Customer

            CreateMap<Record, Personal>()
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Potentialname))
                .ForMember(dest => dest.Gender, src => src.MapFrom(x => x.Cf1026))
                .ForMember(dest => dest.Phone, src => src.MapFrom(x => x.Cf854))
                .ForMember(dest => dest.IdCard, src => src.MapFrom(x => x.Cf1050))
                .ForMember(dest => dest.Email, src => src.MapFrom(x => x.Cf1028))
                .ForMember(dest => dest.CurrentAddress, src => src.MapFrom(x => new Address { FullAddress = x.Cf892 }))
                .ForMember(dest => dest.DateOfBirth, src => src.MapFrom(x => x.Cf948))
                .ForMember(dest => dest.MaritalStatus, src => src.MapFrom(x => x.Cf1030))
                .ForMember(dest => dest.PermanentAddress, src => src.MapFrom(x => new Address { FullAddress = x.Cf1002 }))
                .ForMember(dest => dest.PotentialNo, src => src.MapFrom(x => x.PotentialNo));

            CreateMap<Record, Working>()
                .ForMember(dest => dest.Job, src => src.MapFrom(x => x.Cf1246))
                .ForMember(dest => dest.Income, src => src.MapFrom(x => x.Cf884))
                .ForMember(dest => dest.CompanyName, src => src.MapFrom(x => x.Cf962))
                .ForMember(dest => dest.CompanyAddress, src => src.MapFrom(x => new Address { FullAddress = x.Cf1020 }))
                .ForMember(dest => dest.CompanyPhone, src => src.MapFrom(x => x.Cf976))
                .ForMember(dest => dest.Position, src => src.MapFrom(x => x.Cf982))
                .ForMember(dest => dest.WorkPeriod, src => src.MapFrom(x => x.Cf984))
                .ForMember(dest => dest.TypeOfContract, src => src.MapFrom(x => x.Cf986))
                .ForMember(dest => dest.HealthCardInssurance, src => src.MapFrom(x => x.Cf988));

            CreateMap<Record, Loan>()
                .ForMember(dest => dest.Amount, src => src.MapFrom(x => x.Cf968))
                .ForMember(dest => dest.Product, src => src.MapFrom(x => x.Cf1032))
                .ForMember(dest => dest.RequestDocuments, src => src.MapFrom(x => x.Cf1036))
                .ForMember(dest => dest.Term, src => src.MapFrom(x => x.Cf990))
                .ForMember(dest => dest.GenarateToLead, src => src.MapFrom(x => x.Createdtime))
                .ForMember(dest => dest.FollowedDate, src => src.MapFrom(x => x.Modifiedtime))
                .ForMember(dest => dest.Owner, src => src.MapFrom(x => x.Cf978))
                .ForMember(dest => dest.AppDate, src => src.MapFrom(x => x.Cf992))
                .ForMember(dest => dest.DisbursalDate, src => src.MapFrom(x => x.Cf994));

            CreateMap<Record, Models.Result>()
                .ForMember(dest => dest.Note, src => src.MapFrom(x => x.Description))
                .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Cf1184));

            CreateMap<Record, Counsel>()
                .ForMember(dest => dest.LastCounselling, src => src.MapFrom(x => x.Cf1266))
                .ForMember(dest => dest.ApptSchedule, src => src.MapFrom(x => x.Cf1052))
                .ForMember(dest => dest.TeleSalesCode, src => src.MapFrom(x => x.AssignedUserId.Value))
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.AssignedUserId.Label))
                .ForMember(dest => dest.Campain, src => src.MapFrom(x => x.AssignedUserId.Value))
                .ForMember(dest => dest.Remark, src => src.MapFrom(x => x.Cf1196))
                .ForMember(dest => dest.Occupation, src => src.MapFrom(x => x.Cf1246))
                .ForMember(dest => dest.TeamCode, src => src.MapFrom(x => x.Cf972))
                .ForMember(dest => dest.GroupCode, src => src.MapFrom(x => x.Cf1008));

            CreateMap<Record, Customer>()
                .ForMember(dest => dest.Personal, src => src.MapFrom(x => x))
                .ForMember(dest => dest.Working, src => src.MapFrom(x => x))
                .ForMember(dest => dest.TemporaryAddress, src => src.MapFrom(x => new Address { FullAddress = x.Cf1020 }))
                .ForMember(dest => dest.Loan, src => src.MapFrom(x => x))
                .ForMember(dest => dest.Result, src => src.MapFrom(x => x))
                .AfterMap((src, dest) => dest.UserName = src.Modifiedby.Label.Split("-")[0])
                .ForMember(dest => dest.CRMId, src => src.MapFrom(x => x.Id))
                .ForMember(dest => dest.Route, src => src.MapFrom(x => x.Cf1404))
                .ForMember(dest => dest.Counsel, src => src.MapFrom(x => x))
                .ForMember(dest => dest.Id, src => src.Ignore());

            #endregion

            #region Customer to CRM

            CreateMap<Customer, Record>()
                .ForMember(dest => dest.Potentialname, src => src.MapFrom(x => x.Personal.Name))
                .ForMember(dest => dest.Cf1026, src => src.MapFrom(x => x.Personal.Gender))
                .ForMember(dest => dest.Cf854, src => src.MapFrom(x => x.Personal.Phone))
                .ForMember(dest => dest.Cf1050, src => src.MapFrom(x => x.Personal.IdCard))
                .ForMember(dest => dest.Cf1028, src => src.MapFrom(x => x.Personal.Email))
                .ForMember(dest => dest.Cf892, src => src.MapFrom(x => x.Personal.CurrentAddress.FullAddress))
                .ForMember(dest => dest.Cf948, src => src.MapFrom(x => x.Personal.DateOfBirth))
                .ForMember(dest => dest.Cf1030, src => src.MapFrom(x => x.Personal.MaritalStatus))
                .ForMember(dest => dest.Cf1002, src => src.MapFrom(x => x.Personal.PermanentAddress.FullAddress))
                .ForMember(dest => dest.PotentialNo, src => src.MapFrom(x => x.Personal.PotentialNo))

                .ForMember(dest => dest.Cf1246, src => src.MapFrom(x => x.Working.Job))
                .ForMember(dest => dest.Cf884, src => src.MapFrom(x => x.Working.Income))
                .ForMember(dest => dest.Cf962, src => src.MapFrom(x => x.Working.CompanyName))
                .ForMember(dest => dest.Cf1020, src => src.MapFrom(x => x.Working.CompanyAddress.FullAddress))
                .ForMember(dest => dest.Cf976, src => src.MapFrom(x => x.Working.CompanyPhone))
                .ForMember(dest => dest.Cf982, src => src.MapFrom(x => x.Working.Position))
                .ForMember(dest => dest.Cf984, src => src.MapFrom(x => x.Working.WorkPeriod))
                .ForMember(dest => dest.Cf986, src => src.MapFrom(x => x.Working.TypeOfContract))
                .ForMember(dest => dest.Cf988, src => src.MapFrom(x => x.Working.HealthCardInssurance))

                .ForMember(dest => dest.Cf968, src => src.MapFrom(x => x.Loan.Amount))
                .ForMember(dest => dest.Cf1032, src => src.MapFrom(x => x.Loan.Product))
                .ForMember(dest => dest.Cf1036, src => src.MapFrom(x => x.Loan.RequestDocuments))
                .ForMember(dest => dest.Cf990, src => src.MapFrom(x => x.Loan.Term))
                .ForMember(dest => dest.Createdtime, src => src.MapFrom(x => x.Loan.GenarateToLead))
                .ForMember(dest => dest.Modifiedtime, src => src.MapFrom(x => x.Loan.FollowedDate))
                .ForMember(dest => dest.Cf978, src => src.MapFrom(x => x.Loan.Owner))
                .ForMember(dest => dest.Cf992, src => src.MapFrom(x => x.Loan.AppDate))
                .ForMember(dest => dest.Cf994, src => src.MapFrom(x => x.Loan.DisbursalDate))

                .ForMember(dest => dest.Description, src => src.MapFrom(x => x.Result.Note))
                .ForMember(dest => dest.Cf1184, src => src.MapFrom(x => x.Result.Status))

                .ForMember(dest => dest.Cf1266, src => src.MapFrom(x => x.Counsel.LastCounselling))
                .ForMember(dest => dest.Cf1052, src => src.MapFrom(x => x.Counsel.ApptSchedule))
                .ForMember(dest => dest.AssignedUserId, src => src.MapFrom(x => x.Counsel.TeleSalesCode))
                .ForMember(dest => dest.Cf1196, src => src.MapFrom(x => x.Counsel.Remark))
                .ForMember(dest => dest.Cf1246, src => src.MapFrom(x => x.Counsel.Occupation))
                .ForMember(dest => dest.Cf972, src => src.MapFrom(x => x.Counsel.TeamCode))
                .ForMember(dest => dest.Cf1008, src => src.MapFrom(x => x.Counsel.GroupCode))

                // .ForMember(dest => dest.Modifiedby, src => src.MapFrom(x => new AssignedUserId { Label = x.UserName }))
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x.CRMId))
                .ForMember(dest => dest.Cf1404, src => src.MapFrom(x => x.Route))
                .ForMember(dest => dest.Cf1206, src => src.MapFrom(x => 1));

            #endregion

            #region Customer to MA

            CreateMap<Customer, MARequestDataModel>()
                .ForMember(dest => dest.LEAD_ID, src => src.MapFrom(x => x.Personal.PotentialNo))
                .ForMember(dest => dest.ID_NO, src => src.MapFrom(x => x.Personal.IdCard))
                .ForMember(dest => dest.CONTACT_NAME, src => src.MapFrom(x => x.Personal.Name))
                .ForMember(dest => dest.PHONE, src => src.MapFrom(x => x.Personal.Phone))
                .ForMember(dest => dest.CURRENT_ADDRESS, src => src.MapFrom(x => x.Personal.CurrentAddress.FullAddress))
                .ForMember(dest => dest.PRODUCT, src => src.MapFrom(x => x.Loan.Product))
                .ForMember(dest => dest.T_STATUS_DATE, src => src.MapFrom(x => x.Counsel.LastCounselling))
                .ForMember(dest => dest.APPOINTMENT_DATE, src => src.MapFrom(x => x.Counsel.ApptSchedule))
                .ForMember(dest => dest.APPOINTMENT_ADDRESS, src => src.MapFrom(x => x.Personal.CurrentAddress.FullAddress))
                .ForMember(dest => dest.TSA_IN_CHARGE, src => src.MapFrom(x => x.Counsel.TeleSalesCode))
                .ForMember(dest => dest.TST_TEAM, src => src.MapFrom(x => x.Counsel.TeamCode))
                .ForMember(dest => dest.REQUEST_DOCUMENT, src => src.MapFrom(x => x.Loan.RequestDocuments))
                .ForMember(dest => dest.DOB, src => src.MapFrom(x => x.Personal.DateOfBirth))
                .ForMember(dest => dest.GENDER, src => src.MapFrom(x => x.Personal.Gender))
                .ForMember(dest => dest.COMPANY_NAME, src => src.MapFrom(x => x.Working.CompanyName))
                .ForMember(dest => dest.COMPANY_ADDR, src => src.MapFrom(x => x.Working.CompanyAddress.FullAddress))
                .ForMember(dest => dest.TEL_COMPANY, src => src.MapFrom(x => x.Working.CompanyPhone))
                .ForMember(dest => dest.AREA, src => src.MapFrom(x => x.TemporaryAddress.FullAddress))
                .ForMember(dest => dest.MARITAL_STATUS, src => src.MapFrom(x => x.Personal.MaritalStatus))
                .ForMember(dest => dest.OWNER, src => src.MapFrom(x => x.Loan.Owner))
                .ForMember(dest => dest.INCOME, src => src.MapFrom(x => x.Working.Income))
                .ForMember(dest => dest.POSITION, src => src.MapFrom(x => x.Working.Position))
                .ForMember(dest => dest.WORK_PERIOD, src => src.MapFrom(x => x.Working.WorkPeriod))
                .ForMember(dest => dest.TYPE_OF_CONTRACT, src => src.MapFrom(x => x.Working.TypeOfContract))
                .ForMember(dest => dest.HEALTH_CARD, src => src.MapFrom(x => x.Working.HealthCardInssurance))
                .ForMember(dest => dest.LOAN_AMOUNT, src => src.MapFrom(x => x.Loan.Amount))
                .ForMember(dest => dest.TENURE, src => src.MapFrom(x => x.Loan.Term))
                .ForMember(dest => dest.APP_DATE, src => src.MapFrom(x => x.Loan.AppDate))
                .ForMember(dest => dest.DISBURSAL_DATE, src => src.MapFrom(x => x.Loan.DisbursalDate))
                .ForMember(dest => dest.GENERATE_TO_LEAD, src => src.MapFrom(x => x.Loan.GenarateToLead))
                .ForMember(dest => dest.FOLLOWED_DATE, src => src.MapFrom(x => x.Loan.FollowedDate))
                .ForMember(dest => dest.PERMANENT_ADDR, src => src.MapFrom(x => x.Personal.PermanentAddress.FullAddress))
                .ForMember(dest => dest.TSA_NAME, src => src.MapFrom(x => x.Counsel.Name))
                .ForMember(dest => dest.TSA_CAMPAIN, src => src.MapFrom(x => x.Counsel.Campain))
                .ForMember(dest => dest.TSA_GROUP, src => src.MapFrom(x => x.Counsel.GroupCode))
                .ForMember(dest => dest.TSA_LAST_NOTES, src => src.MapFrom(x => x.Counsel.Remark))
                .ForMember(dest => dest.OCCUPATION, src => src.MapFrom(x => x.Counsel.Occupation))
                .ForMember(dest => dest.ROUTE, src => src.MapFrom(x => x.Route));

            #endregion

            #region CRM to LeadCRM

            CreateMap<AssignedUserId, LeadCrmUser>();

            CreateMap<Record, LeadCrm>()
                .ForMember(dest => dest.LeadCrmId, src => src.MapFrom(x => x.Id))
                .ForMember(dest => dest.Id, src => src.Ignore());

            #endregion

            #region LeadCRM to CRM

            CreateMap<LeadCrmUser, AssignedUserId>();

            CreateMap<LeadCrm, CRMRequestDto>()
                .ForMember(dest => dest.Id, src => src.MapFrom(x => x.LeadCrmId))
                .ForMember(dest => dest.AssignedUserId, src => src.MapFrom(x => x.AssignedUserId != null ? x.AssignedUserId.Value : "-"))
                .ForMember(dest => dest.Cf1190, src => src.MapFrom(x => x.GetStatusMessage()))
                .ForMember(dest => dest.Cf1174, src => src.MapFrom(x => x.PostbackMA != null ? x.PostbackMA.DcLastNote : x.Cf1174))
                .ForMember(dest => dest.Cf884, src => src.MapFrom(x => x.Cf884 ?? "-"))
                .ForMember(dest => dest.Cf892, src => src.MapFrom(x => x.Cf892 ?? "-"))
                .ForMember(dest => dest.Cf990, src => src.MapFrom(x => x.Cf990 ?? "-"))
                .ForMember(dest => dest.Cf1002, src => src.MapFrom(x => x.Cf1002 ?? "-"))
                .ForMember(dest => dest.Cf1188, src => src.MapFrom(x => x.Cf1188 ?? "-"))
                .ForMember(dest => dest.Cf1196, src => src.MapFrom(x => x.Cf1196 ?? "-"))
                .ForMember(dest => dest.Cf1210, src => src.MapFrom(x => x.Cf1210 ?? "-"))
                .ForMember(dest => dest.Cf1264, src => src.MapFrom(x => x.Cf1264 ?? "-"))
                .ForMember(dest => dest.Cf1404, src => src.MapFrom(x => x.Cf1404 ?? "-"))
                .ForMember(dest => dest.Cf968, src => src.MapFrom(x => x.Cf968 ?? "-"))
                .ForMember(dest => dest.Cf1026, src => src.MapFrom(x => x.Cf1026 ?? "-"))
                .ForMember(dest => dest.Cf1036, src => src.MapFrom(x => x.Cf1036 ?? "-"))
                .ForMember(dest => dest.Cf1040, src => src.MapFrom(x => x.Cf1040 ?? "-"))
                .ForMember(dest => dest.Cf1178, src => src.MapFrom(x => x.Cf1178 ?? "-"));

            #endregion

            #region LeadCRM to MA

            CreateMap<LeadCrm, MARequestDataModel>()
                .ForMember(dest => dest.LEAD_ID, src => src.MapFrom(x => x.PotentialNo))
                .ForMember(dest => dest.ID_NO, src => src.MapFrom(x => x.Cf1050))
                .ForMember(dest => dest.CONTACT_NAME, src => src.MapFrom(x => x.Potentialname))
                .ForMember(dest => dest.PHONE, src => src.MapFrom(x => x.Cf854))
                .ForMember(dest => dest.CURRENT_ADDRESS, src => src.MapFrom(x => x.Cf892))
                .ForMember(dest => dest.PRODUCT, src => src.MapFrom(x => x.Cf1032))
                .ForMember(dest => dest.T_STATUS_DATE, src => src.MapFrom(x => x.Cf1266))
                .ForMember(dest => dest.APPOINTMENT_DATE, src => src.MapFrom(x => x.Cf1052))
                .ForMember(dest => dest.APPOINTMENT_ADDRESS, src => src.MapFrom(x => x.Cf892))
                .ForMember(dest => dest.TSA_IN_CHARGE, src => src.MapFrom(x => "24H-TE0001"))
                .ForMember(dest => dest.TST_TEAM, src => src.MapFrom(x => x.Cf972))
                .ForMember(dest => dest.REQUEST_DOCUMENT, src => src.MapFrom(x => x.Cf1036))
                .ForMember(dest => dest.DOB, src => src.MapFrom(x => x.Cf948))
                .ForMember(dest => dest.GENDER, src => src.MapFrom(x => x.Cf1026))
                .ForMember(dest => dest.COMPANY_NAME, src => src.MapFrom(x => x.Cf962))
                .ForMember(dest => dest.COMPANY_ADDR, src => src.MapFrom(x => x.Cf1020))
                .ForMember(dest => dest.TEL_COMPANY, src => src.MapFrom(x => x.Cf976))
                .ForMember(dest => dest.AREA, src => src.MapFrom(x => x.Cf1020))
                .ForMember(dest => dest.MARITAL_STATUS, src => src.MapFrom(x => x.Cf1030))
                .ForMember(dest => dest.OWNER, src => src.MapFrom(x => x.Cf978))
                .ForMember(dest => dest.INCOME, src => src.MapFrom(x => x.Cf884))
                .ForMember(dest => dest.POSITION, src => src.MapFrom(x => x.Cf982))
                .ForMember(dest => dest.WORK_PERIOD, src => src.MapFrom(x => x.Cf984))
                .ForMember(dest => dest.TYPE_OF_CONTRACT, src => src.MapFrom(x => x.Cf986))
                .ForMember(dest => dest.HEALTH_CARD, src => src.MapFrom(x => x.Cf988))
                .ForMember(dest => dest.LOAN_AMOUNT, src => src.MapFrom(x => x.Cf968))
                .ForMember(dest => dest.TENURE, src => src.MapFrom(x => x.Cf990))
                .ForMember(dest => dest.APP_DATE, src => src.MapFrom(x => x.Cf992))
                .ForMember(dest => dest.DISBURSAL_DATE, src => src.MapFrom(x => x.Cf994))
                .ForMember(dest => dest.GENERATE_TO_LEAD, src => src.MapFrom(x => x.Createdtime))
                .ForMember(dest => dest.FOLLOWED_DATE, src => src.MapFrom(x => x.Modifiedtime))
                .ForMember(dest => dest.PERMANENT_ADDR, src => src.MapFrom(x => x.Cf1002))
                .ForMember(dest => dest.TSA_NAME, src => src.MapFrom(x => x.GetSalesStaffName()))
                .ForMember(dest => dest.TSA_CAMPAIN, src => src.MapFrom(x => "24H-TE0001"))
                .ForMember(dest => dest.TSA_GROUP, src => src.MapFrom(x => x.Cf1008))
                .ForMember(dest => dest.TSA_LAST_NOTES, src => src.MapFrom(x => x.Cf1196))
                .ForMember(dest => dest.OCCUPATION, src => src.MapFrom(x => x.Cf1246))
                .ForMember(dest => dest.ROUTE, src => src.MapFrom(x => x.Cf1404));

            #endregion

            #region MA to LeadCRM

            CreateMap<MAPostBackRequestModel, PostbackMA>();

            CreateMap<MAPostBackRequestModel, LeadCrm>()
                .ForMember(dest => dest.Cf1184, src => src.MapFrom(x => x.GetStatusMessage()))
                .ForMember(dest => dest.Description, src => src.MapFrom(x => x.GetDetailStatusMessage()))
                .ForMember(dest => dest.PostbackMA, src => src.MapFrom(x => x));

            #endregion

            CreateMap<GetCaseRequestDto, GetCaseMCRequestDto>()
                .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Status.ToString()));


            #region Checklist
            CreateMap<GroupDtoModel, GroupDocument>();
            CreateMap<DocumentDtoModel, DocumentUpload>();
            #endregion

            CreateMap<MCCheckCICInfoResponseDto, MCCheckCICModel>();

            CreateMap<FIBOResquestDto, LeadCrm>()
                .ForMember(dest => dest.Potentialname, src => src.MapFrom(x => x.ContactName))
                .ForMember(dest => dest.Cf854, src => src.MapFrom(x => x.Phone))
                .ForMember(dest => dest.Cf1050, src => src.MapFrom(x => x.Cmnd));

        }
    }
}
