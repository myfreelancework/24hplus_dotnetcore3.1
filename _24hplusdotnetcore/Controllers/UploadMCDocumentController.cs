using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using _24hplusdotnetcore.Services.MC;
using _24hplusdotnetcore.Models;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class UploadMCDocumentController : ControllerBase
    {
        private readonly MCService _mcServices;
        private readonly ILogger<UploadMCDocumentController> _logger;
        public UploadMCDocumentController(ILogger<UploadMCDocumentController> logger, MCService mcServices)
        {
            _logger = logger;
            _mcServices = mcServices;
        }

        [Route("api/upload-mc-doc")]
        [HttpPost]
        public ActionResult<dynamic> UploadDocumentMC()
        {
            try
            {
                _mcServices.PushDataToMC();
                return Ok("Data uploaded!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
    }
}