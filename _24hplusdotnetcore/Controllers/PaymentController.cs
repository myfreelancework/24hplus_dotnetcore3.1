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
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly PaymentServices _paymentServices;
        public PaymentController(ILogger<PaymentController> logger, PaymentServices paymentServices)
        {
            _logger = logger;
            _paymentServices = paymentServices;
        }
        [HttpGet]
        [Route("api/getpayment")]
        public ActionResult<PagingDataResponse> GetPayments([FromQuery] int? pagesize, [FromQuery] int? pagenumber)
        {
            try
            {
                int totalpage = 0;
                var lstpayment = _paymentServices.GetPayments(pagenumber, pagesize, ref totalpage);
                return Ok(new PagingDataResponse
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstpayment,
                    pagenumber = pagenumber.HasValue? (int)pagenumber : 1,
                    totalpage = totalpage
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