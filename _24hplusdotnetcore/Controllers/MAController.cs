using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using _24hplusdotnetcore.Services.CRM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    [Route("api/ma")]
    public class MAController : ControllerBase
    {
        private readonly ILogger<MAController> _logger;
        private readonly CustomerServices _customerServices;
        private readonly DataCRMProcessingServices _dataCRMProcessingServices;

        public MAController(
            ILogger<MAController> logger,
            CustomerServices customerServices,
            DataCRMProcessingServices dataCRMProcessingServices)
        {
            _logger = logger;
            _customerServices = customerServices;
            _dataCRMProcessingServices = dataCRMProcessingServices;
        }

        [HttpPost("postback")]
        public async Task<ActionResult<ResponseContext>> PostBackAsync(MAPostBackRequestModel mAPostBack)
        {
            try
            {
                var customer = await _customerServices.GetByCrmIdAsync(mAPostBack.LeadId);
                if (customer == null)
                {
                    return Ok(new ResponseMAContext
                    {
                        Result = false,
                        Message = "Customer does not exist",
                    });
                }

                await _customerServices.UpdateAsync(mAPostBack);

                _dataCRMProcessingServices.CreateOne(new Models.CRM.DataCRMProcessing
                {
                    CustomerId = customer.Id,
                    Status = DataCRMProcessingStatus.InProgress,
                    LeadSource = LeadSourceType.MA.ToString()
                });

                return Ok(new ResponseMAContext
                {
                    Result = true,
                    Message = "",
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
