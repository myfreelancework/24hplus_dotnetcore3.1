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
    public class ConfigController : ControllerBase
    {
        private readonly ILogger<ConfigController> _logger;
        public ConfigController(ILogger<ConfigController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        [Route("api/config/banner")]
        public ActionResult<ResponseContext> GetBanner()
        {
            try
            {
                String[] a = {
                    "https://photos.app.goo.gl/Czfr2A2BeDyZE6Zs9",
                    "https://photos.app.goo.gl/a7We41Htuz1mYesm8",
                    "https://photos.app.goo.gl/3uTfrocUKowtmQdVA",
                    "https://photos.app.goo.gl/npKYnUPja7i3tHDGA"};

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = a
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