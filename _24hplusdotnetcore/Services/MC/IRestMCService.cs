using _24hplusdotnetcore.ModelDtos;
using Refit;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MC
{
    public interface IRestMCService
    {
        [Get("/mcMobileService/service/v1.0/mobile-4sales/check-list")]
        [Headers("Authorization: Bearer")]
        Task<CustomerCheckListResponseModel> CheckListAsync([Query] CustomerCheckListRequestModel customerCheckListRequestModel);
    }
}
