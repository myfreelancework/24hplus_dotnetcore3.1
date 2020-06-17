using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using _24hplusdotnetcore.Services.MC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

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
    }
}