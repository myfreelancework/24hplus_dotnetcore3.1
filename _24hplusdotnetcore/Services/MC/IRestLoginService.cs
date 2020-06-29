using _24hplusdotnetcore.ModelDtos;
using Refit;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MC
{
    public interface IRestLoginService
    {
        [Post("/mcMobileService/service/v1.0/authorization")]
        Task<LoginResponseModel> GetTokenAsync([Body] LoginRequestModel body);
    }
}
