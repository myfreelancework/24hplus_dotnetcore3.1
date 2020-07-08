using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.CRM;
using _24hplusdotnetcore.Services;
using _24hplusdotnetcore.Services.CRM;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
        private readonly DataCRMProcessingServices _dataCRMProcessingServices;
        private readonly LeadCrmService _leadCrmService;
        private readonly IMapper _mapper;

        public MAController(
            ILogger<MAController> logger,
            DataCRMProcessingServices dataCRMProcessingServices,
            LeadCrmService leadCrmService,
            IMapper mapper)
        {
            _logger = logger;
            _dataCRMProcessingServices = dataCRMProcessingServices;
            _leadCrmService = leadCrmService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("postback")]
        public async Task<ActionResult<ResponseContext>> PostBackAsync(MAPostBackRequestModel mAPostBack)
        {
            try
            {
                LeadCrm leadCrm = await _leadCrmService.GetByPotentialNoAsync(mAPostBack.LeadId);
                if (leadCrm == null)
                {
                    return Ok(new ResponseMAContext
                    {
                        Result = false,
                        Message = "Lead does not exist",
                    });
                }

                _mapper.Map(mAPostBack, leadCrm);
                await _leadCrmService.ReplaceOneAsync(leadCrm);

                _dataCRMProcessingServices.CreateOne(new DataCRMProcessing
                {
                    LeadCrmId = leadCrm.Id,
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
