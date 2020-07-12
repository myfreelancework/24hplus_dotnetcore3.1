using _24hplusdotnetcore.ModelDtos;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MC
{
    public interface IRestMCService
    {
        [Get("/mcMobileService/service/v1.0/mobile-4sales/check-list")]
        [Headers("Authorization: Bearer")]
        Task<CustomerCheckListResponseModel> CheckListAsync([Query] CustomerCheckListRequestModel customerCheckListRequestModel);
        
        [Get("/mcMobileService/service/v1.0/mobile-4sales/kiosks")]
        [Headers("Authorization: Bearer")]
        Task<IEnumerable<KiosModel>> GetKiosAsync();

        [Post("/mcMobileService/service/v1.0/mobile-4sales/cancel-case")]
        [Headers("Authorization: Bearer")]
        Task<MCCancelCaseResponseDto> CancelCaseAsync(MCCancelCaseRequestDto cancelCaseRequestDto);
    }
}
