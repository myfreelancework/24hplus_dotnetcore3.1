using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.GCC;
using _24hplusdotnetcore.Services;
using _24hplusdotnetcore.Services.GCC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _24hplusdotnetcore.Controllers
{

    [ApiController]
    public class GCCController : ControllerBase
    {
        private readonly ILogger<GCCController> _logger;
        private readonly GCCService _gccService;
        private readonly GCCProductService _gccProductService;
        public GCCController(ILogger<GCCController> logger, GCCService gccService, GCCProductService gccProductService)
        {
            _logger = logger;
            _gccService = gccService;
            _gccProductService = gccProductService;
        }


        [HttpPost]
        [Route("api/gcc/personal")]
        public ActionResult<ResponseContext> PersonalInsurance(GCCPersonalInsurance body)
        {
            try
            {
                var response = new ResponseContext
                {
                    code = (int)Common.ResponseCode.ERROR,
                    message = "",
                    data = null
                };
                var product = _gccProductService.FindOneByProductName(body.product_name);
                if (product != null)
                {
                    var currPackage = product.package.Split(";");

                    body.state = GccState.NEW;
                    body.program = product.program;
                    body.agency_id = product.agency_id;
                    body.product_code = product.product_code;
                    body.buy_gender = body.buy_gender == "Nam" ? "m" : "f";
                    body.begin_date = DateTime.Today.ToString("yyyy-MM-dd");
                    body.request_code = ConfigRequest.GCC_REQUEST_CODE + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    body.url_callback = ConfigRequest.GCC_CALLBACK + "?requestCode=" + body.request_code;
                    if (currPackage.Length == 1)
                    {
                        body.package = product.package;
                    }
                    else
                    {
                        body.package = _gccProductService.MappingPackage(body.package);
                    }

                    var currPerson = _gccService.CreateOne(body);
                    if (currPerson != null)
                    {
                        var token = _gccService.GetToken();
                        if (!string.IsNullOrEmpty(token))
                        {
                            var sendObj = _gccService.SendInfo(token, currPerson);
                            if (sendObj != null)
                            {
                                switch ((int)sendObj.code)
                                {
                                    case 200:
                                        currPerson.state = GccState.SENT_TO_GCC_SUCCEESS;
                                        response.code = (int)Common.ResponseCode.SUCCESS;
                                        response.message = GCCMessage.SUCCESS;
                                        break;
                                    case 201:
                                        currPerson.state = GccState.SENT_TO_GCC_ERROR;
                                        currPerson.message = response.message = GCCMessage.RESPONSE_DUPLICATE;
                                        break;
                                    case 400:
                                        currPerson.state = GccState.SENT_TO_GCC_ERROR;
                                        currPerson.message = response.message = string.Format(GCCMessage.RESPONSE_ERROR, sendObj.message);
                                        break;
                                }

                                _gccService.UpdateOne(currPerson);
                            }
                            else
                            {
                                response.message = GCCMessage.CANT_PUSH_DATA;
                                currPerson.state = GccState.SENT_TO_GCC_ERROR;
                                currPerson.message = GCCMessage.CANT_PUSH_DATA;
                                _gccService.UpdateOne(currPerson);
                            }

                        }
                        else
                        {
                            response.message = GCCMessage.CANT_GET_KEY;
                        }

                    }
                    else
                    {
                        response.message = GCCMessage.CANT_SAVE;
                    }

                }
                else
                {
                    response.message = GCCMessage.NOT_FIND_PRODUCT;
                }

                return Ok(response);
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
        [Route("api/gcc/postbackPersonal")]
        public ActionResult<ResponseContext> PersonalInsurancePostback([FromQuery] string request_code, [FromQuery] bool status, [FromQuery] string link)
        {
            try
            {
                var curPerson = _gccService.FindOneByRequestCode(request_code);
                curPerson.status = status;
                curPerson.link = link;
                _gccService.UpdateOne(curPerson);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = "",
                    data = null
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