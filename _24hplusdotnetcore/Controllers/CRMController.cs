﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services.CRM;
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
        public CRMController(ILogger<CRMController> logger, CRMServices crmServices)
        {
            _logger = logger;
            _crmService = crmServices;
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
    }
}