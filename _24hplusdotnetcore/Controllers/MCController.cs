using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using _24hplusdotnetcore.Services.MC;
using Microsoft.AspNetCore.Authorization;
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
        private readonly MCNotificationService _mcNotificationService;
        private readonly MCCheckCICService _mcCheckCICService;
        public MCController(ILogger<MCController> logger, 
        MCService mcService,
        MCCheckCICService mcCheckCICService,
        MCNotificationService mcNotificationService)
        {
            _logger = logger;
            _mcService = mcService;
            _mcCheckCICService = mcCheckCICService;
            _mcNotificationService = mcNotificationService;
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

                if (result?.CheckList?.Any() != true)
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


        [HttpPost]
        [Route("api/mc/notification")]
        public ActionResult<ResponseContext> PushNotification(MCNotificationDto noti)
        {
            try
            {
                _mcNotificationService.CreateOne(noti);
                return Ok(new ResponseMCContext
                {
                    ReturnCode = "200",
                    ReturnMes = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new ResponseMCContext
                {
                    ReturnCode = "400",
                    ReturnMes = "Error"
                });
            }
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("api/mc/update-cic")]
        public async Task<ActionResult<ResponseContext>> UpdateCICAsync(MCUpdateCICDto dto)
        {
            try
            {
                var oldCic = _mcCheckCICService.FindOneByRequestId(dto.RequestId);
                if (oldCic != null)
                {
                    oldCic.CicResult = dto.CicResult;
                    oldCic.Description = dto.Description;
                    oldCic.CicImageLink = dto.CicImageLink;
                    oldCic.LastUpdateTime = dto.LastUpdateTime;
                    oldCic.Status = dto.Status;
                    await _mcCheckCICService.ReplaceOneAsync(oldCic);
                }
                else {
                    return Ok(new ResponseMCContext
                    {
                        ReturnCode = "400",
                        ReturnMes = "Không tìm thấy RequestId"
                    });
                }
                return Ok(new ResponseMCContext
                {
                    ReturnCode = "200",
                    ReturnMes = ""
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(new ResponseMCContext
                {
                    ReturnCode = "400",
                    ReturnMes = "Error"
                });
            }
        }
        
        [HttpGet]
        [Route("api/mc/push-mc")]
        public ActionResult<ResponseContext> CheckListAsync()
        {
            try
            {
                _mcService.PushDataToMC();
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = null
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