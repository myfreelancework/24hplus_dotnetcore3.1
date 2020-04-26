using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class MobileVersionController : ControllerBase
    {
        private readonly ILogger<MobileVersionController> _logger;
        private readonly MobileVersionServices _mobileVersionServices;
        public MobileVersionController(ILogger<MobileVersionController> logger, MobileVersionServices mobileVersionServices)
        {
            _logger = logger;
            _mobileVersionServices = mobileVersionServices;
        }
       [HttpGet]
       [Route("api/checkversion")]
       public ActionResult<ResponseContext> CheckVersion([FromQuery]string type, [FromQuery]string version)
        {
            try
            {
                var currentVersion = _mobileVersionServices.GetMobileVersion(type, version);
                if (currentVersion != null)
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.SUCCESS,
                        message = Common.Message.SUCCESS,
                        data = currentVersion
                    });
                }
                else
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = Common.Message.VERSION_IS_OLD,
                        data = currentVersion
                    });
                }
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