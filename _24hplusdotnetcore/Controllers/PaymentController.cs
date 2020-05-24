using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly ProductServices _productServices;
        public PaymentController(ILogger<PaymentController> logger, PaymentServices paymentServices, ProductServices productServices)
        {
            _logger = logger;
            _paymentServices = paymentServices;
            _productServices = productServices;
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
                    pagenumber = pagenumber.HasValue ? (int)pagenumber : 1,
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

        [HttpPost]
        [Route("api/payment/calc")]
        public ActionResult<ResponseContext> CalculatorPayment(PaymentCalc paymentCalc)
        {
            try
            {
                var objProduct = new Product();
                objProduct = _productServices.GetProductByProductId(paymentCalc.product);
                if (objProduct != null)
                {
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    DateTime firstDay = DateTime.ParseExact(paymentCalc.dateReceive, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider);
                    double part1 = 1;
                    double part2 = 1;

                    for (int i = 0; i < paymentCalc.term; i++)
                    {
                        firstDay.AddMonths(1);
                        DateTime dec31 = new DateTime(firstDay.Year, 12, 31);
                        int daysInMonth = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);
                        double monthlyInterest = Math.Round(double.Parse(objProduct.InterestRateByYear) * daysInMonth / dec31.DayOfYear, 2, MidpointRounding.ToEven);
                        part1 = Math.Round((1 + monthlyInterest / 100) * part1, 4, MidpointRounding.ToEven);
                        if (i != 0)
                        {
                            part2 = Math.Round(part2 * (1 + monthlyInterest / 100) + 1, 4, MidpointRounding.ToEven);
                        }
                    }
                    double paymentMonthly = part1 * paymentCalc.amountLoan / part2;
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.SUCCESS,
                        message = Common.Message.SUCCESS,
                        data = new { paymentMonthly = Math.Round(paymentMonthly), firstDate = firstDay.AddMonths(1).ToString("dd/MM/yyyy") }
                    });
                }
                else
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = Common.Message.NOT_FOUND_PRODUCT,
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