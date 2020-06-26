using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class CheckInfoController : ControllerBase
    {
        private readonly ILogger<CheckInfoController> _logger;
        private readonly CheckInfoServices _checkInforServices;
        public CheckInfoController(ILogger<CheckInfoController> logger, CheckInfoServices checkInforServices)
        {
            _logger = logger;
            _checkInforServices = checkInforServices;
        }
        [HttpGet]
        [Route("api/checkinfo")]
        public ActionResult<ResponseContext> CheckInfo([FromQuery] string greentype, [FromQuery] string citizenId, [FromQuery] string customerName)
        {
            try
            {
                var response = _checkInforServices.CheckInfoByType(greentype, citizenId, customerName);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/checkduplicate")]
        public ActionResult<ResponseContext> CheckDuplicate([FromQuery] string greentype, [FromQuery] string citizenId)
        {
            try
            {
                var response = _checkInforServices.CheckDuplicateByType(greentype, citizenId);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = response.returnMes,
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/checkcat")]
        public ActionResult<ResponseContext> CheckCat([FromQuery] string greentype, [FromQuery] string companyTaxNumber)
        {
            try
            {
                //dynamic response = new {
                //compName = "CÔNG TY TNHH EB CẦN THƠ",
                //catType = "CAT B",
                //compAddrStreet = "LÔ SỐ 1, KDC HƯNG PHÚ 1, PHƯỜNG HƯNG PHÚ, QUẬN CÁI RĂNG, TP CẦN THƠ",
                //officeNumber = "",
                //companyTaxNumber = "1801210593",
                //};
                dynamic response = _checkInforServices.CheckCAT(greentype, companyTaxNumber);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("api/check-list")]
        public async Task<ActionResult<ResponseContext>> CheckListAsync([FromQuery] string customerId)
        {
            try
            {
                CustomerCheckListResponseModel result = await _checkInforServices.CheckListAsync(customerId);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }
    }
}