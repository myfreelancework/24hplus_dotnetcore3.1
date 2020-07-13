using _24hplusdotnetcore.ModelDtos;
using Refit;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MA
{
    public interface IRestMAService
    {
        [Post("/production/api/ReceiveLeads")]
        Task<MAResponseModel> PushCustomerAsync([Body] MARequestModel body);
    }
}
