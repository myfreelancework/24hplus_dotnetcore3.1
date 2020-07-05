using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using _24hplusdotnetcore.Services.MC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class MCController : ControllerBase
    {
        private readonly ILogger<MCController> _logger;
        private readonly MCService _mcService;
        public MCController(ILogger<MCController> logger, MCService mcService)
        {
            _logger = logger;
            _mcService = mcService;
        }

        [HttpGet]
        [Route("api/getmcproduct")]
        public ActionResult<dynamic> GetMCProduct()
        {
            try
            {
                List<MCProduct> lstMCProduct = new List<MCProduct>();
                lstMCProduct = _mcService.GetProduct();
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstMCProduct
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }

        [HttpGet]
        [Route("api/check-list")]
        public async Task<ActionResult<ResponseContext>> CheckListAsync([FromQuery] string customerId)
        {
            try
            {
                CustomerCheckListResponseModel result = await _mcService.CheckListAsync(customerId);
                
                if(result?.CheckList?.Any() != true)
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = Common.Message.NOT_FOUND_PRODUCT,
                        data = result
                    });
                }

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
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.ERROR,
                    message = ex.Message,
                });
            }
        }

        [HttpGet]
        [Route("api/kios")]
        public async Task<ActionResult<ResponseContext>> GetKiosAsync()
        {
            try
            {
                IEnumerable<KiosModel> kios = await _mcService.GetKiosAsync();

                if (kios?.Any() != true)
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = Common.Message.NOT_FOUND_KIOS,
                        data = kios
                    });
                }

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = kios
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.ERROR,
                    message = ex.Message,
                });
            }
        }
    }
}