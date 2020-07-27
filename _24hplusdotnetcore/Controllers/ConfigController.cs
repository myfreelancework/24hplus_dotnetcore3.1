using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using _24hplusdotnetcore.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly ILogger<ConfigController> _logger;
        private readonly MCConfig _mCConfig;
        private readonly ConfigServices _configService;
        public ConfigController(ILogger<ConfigController> logger, 
        IOptions<MCConfig> mCConfigOptions,
        ConfigServices configService)
        {
            _logger = logger;
            _configService = configService;
            _mCConfig = mCConfigOptions.Value;
        }
        [HttpGet]
        [Route("api/config/banner")]
        public ActionResult<ResponseContext> GetBanner()
        {
            try
            {

                var banner = _configService.FindOneByKey("Banner");
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = banner.Value
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

        [AllowAnonymous]
        [HttpGet]
        [Route("api/config/env")]
        public ActionResult<string> CheckEnv()
        {
            try
            {

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = _mCConfig.Host
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