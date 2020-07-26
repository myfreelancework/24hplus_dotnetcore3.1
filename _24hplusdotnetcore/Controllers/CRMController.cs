using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _24hplusdotnetcore.Common.Attributes;
using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.CRM;
using _24hplusdotnetcore.Services;
using _24hplusdotnetcore.Services.CRM;
using AutoMapper;
using DnsClient.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.Fluent;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    [Route("api/crm")]
    public class CRMController : ControllerBase
    {
        private readonly ILogger<CRMController> _logger;
        private readonly CRMServices _crmService;
        private readonly DataCRMProcessingServices _dataCRMProcessingServices;
        private readonly LeadCrmService _leadCrmService;
        private readonly IMapper _mapper;

        public CRMController(
            ILogger<CRMController> logger, 
            CRMServices crmServices,
            DataCRMProcessingServices dataCRMProcessingServices,
            LeadCrmService leadCrmService,
            IMapper mapper)
        {
            _logger = logger;
            _crmService = crmServices;
            _dataCRMProcessingServices = dataCRMProcessingServices;
            _leadCrmService = leadCrmService;
            _mapper = mapper;
        }

        [HttpPost("pullnewcustomers")]
        public ActionResult<dynamic> AddNewCustomerFromCRM()
        {
            try
            {
                var insertCount = _crmService.GetCustomerFromCRM();
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = insertCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }


        /// <summary>
        /// Run job push Customer to CRM
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("push-data")]
        public ActionResult RunJobPushCustomerToCRM()
        {
            try
            {
                Task.Factory.StartNew(() => _crmService.AddingDataToCRM());
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }

        [FIBOAuthorize]
        [HttpPost("push-data")]
        public async Task<ActionResult> PushCustomerAsync(FIBOResquestDto fIBOResquestDto)
        {
            try
            {
                var leadCrm = _mapper.Map<LeadCrm>(fIBOResquestDto);
                await _leadCrmService.InsertAsync(leadCrm);

                _dataCRMProcessingServices.CreateOne(new DataCRMProcessing
                {
                    LeadCrmId = leadCrm.Id,
                    LeadSource = LeadSourceType.FIBO.ToString()
                });

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS
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