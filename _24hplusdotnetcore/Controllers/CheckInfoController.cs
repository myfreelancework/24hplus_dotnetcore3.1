using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<ResponseContext>> CheckInfo([FromQuery] string greentype, [FromQuery] string citizenId, [FromQuery] string customerName)
        {
            try
            {
                var response = await _checkInforServices.CheckInfoByTypeAsync(greentype, citizenId, customerName);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = response
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.ERROR,
                    message = ex.Message,
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
        public async Task<ActionResult<ResponseContext>> CheckDuplicate([FromQuery] string greentype, [FromQuery] string citizenId)
        {
            try
            {
                var response = await _checkInforServices.CheckDuplicateByTypeAsync(greentype, citizenId);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = response?.ReturnMes,
                    data = response
                });
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.ERROR,
                    message = ex.Message,
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
        public async Task<ActionResult<ResponseContext>> CheckCatAsync([FromQuery] string greentype, [FromQuery] string companyTaxNumber)
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
                MCCheckCatResponseDto response = await _checkInforServices.CheckCatAsync(greentype, companyTaxNumber);
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

    }
}