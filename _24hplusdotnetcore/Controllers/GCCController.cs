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
        private readonly GCCMotoService _gccMotoService;
        private readonly GCCMotoProgramService _gccMotoProgramService;
        private readonly GCCProductService _gccProductService;
        public GCCController(ILogger<GCCController> logger,
                            GCCService gccService,
                            GCCMotoService gccMotoService,
                            GCCProductService gccProductService,
                            GCCMotoProgramService gccMotoProgramService)
        {
            _logger = logger;
            _gccService = gccService;
            _gccMotoService = gccMotoService;
            _gccProductService = gccProductService;
            _gccMotoProgramService = gccMotoProgramService;
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
                                        currPerson.link = sendObj.result.payment;
                                        response.data = new
                                        {
                                            link = sendObj.result.payment
                                        };
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


        [HttpPost]
        [Route("api/gcc/moto")]
        public ActionResult<ResponseContext> MotoInsurance(GCCMotoInsuranceModel body)
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
                    var currProgram = _gccMotoProgramService.FindProgramByTitle(body.programTitle);
                    var currMoto = _gccMotoProgramService.FindMotoByName(body.package.motoName);

                    if (currProgram != null && currMoto != null)
                    {
                        body.state = GccState.NEW;
                        body.program = currProgram.name;
                        body.agency_id = product.agency_id;
                        body.product_code = product.product_code;
                        body.package.motorcycle_id = currMoto.motoId.ToString();
                        body.buy_gender = body.buy_gender == "Nam" ? "m" : "f";
                        body.request_code = ConfigRequest.GCC_REQUEST_CODE + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        body.url_callback = ConfigRequest.GCC_CALLBACK + "?requestCode=" + body.request_code;

                        var currPerson = _gccMotoService.CreateOne(body);
                        if (currPerson != null)
                        {
                            var token = _gccService.GetToken();
                            if (!string.IsNullOrEmpty(token))
                            {
                                var sendObj = _gccMotoService.SendInfo(token, currPerson);
                                if (sendObj != null)
                                {
                                    switch ((int)sendObj.code)
                                    {
                                        case 200:
                                            currPerson.state = GccState.SENT_TO_GCC_SUCCEESS;
                                            currPerson.link = sendObj.result.payment;
                                            response.data = new
                                            {
                                                link = sendObj.result.payment
                                            };
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

                                    _gccMotoService.UpdateOne(currPerson);
                                }
                                else
                                {
                                    response.message = GCCMessage.CANT_PUSH_DATA;
                                    currPerson.state = GccState.SENT_TO_GCC_ERROR;
                                    currPerson.message = GCCMessage.CANT_PUSH_DATA;
                                    _gccMotoService.UpdateOne(currPerson);
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
                        if (currMoto == null)
                        {
                            response.message = string.Format(GCCMessage.NOT_FIND_KIND_OF_MOTO, body.package.motoName);
                        }
                        else
                        {
                            response.message = string.Format(GCCMessage.NOT_FIND_PROGRAM, body.programTitle);
                        }
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
        public ActionResult<ResponseContext> PersonalInsurancePostback([FromQuery] string requestCode, [FromQuery] string status, [FromQuery] string link)
        {
            try
            {
                if (status != "")
                {
                    var curPerson = _gccService.FindOneByRequestCode(requestCode);
                    var curMoto = _gccMotoService.FindOneByRequestCode(requestCode);
                    if (curPerson != null || curMoto != null)
                    {
                        if (curPerson != null)
                        {
                            if (status == "true" || status == "1")
                            {
                                curPerson.status = true;
                            }
                            else
                            {
                                curPerson.status = false;
                            }
                            if (link != "")
                            {
                                curPerson.link = link;
                            }
                            curPerson.state = GccState.RECEIVE_POSTBACK;
                            _gccService.UpdateOne(curPerson);
                        }
                        if (curMoto != null)
                        {
                            if (status == "true" || status == "1")
                            {
                                curMoto.status = true;
                            }
                            else
                            {
                                curMoto.status = false;
                            }
                            if (link != "")
                            {
                                curMoto.link = link;
                            }
                            curMoto.state = GccState.RECEIVE_POSTBACK;
                            _gccMotoService.UpdateOne(curMoto);
                        }

                        return Ok(new ResponseContext
                        {
                            code = (int)Common.ResponseCode.SUCCESS,
                            message = "",
                            data = null
                        });
                    }
                    else
                    {
                        return Ok(new ResponseContext
                        {
                            code = (int)Common.ResponseCode.ERROR,
                            message = "Không tìm thấy request code",
                            data = null
                        });
                    }
                }
                else
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = "Không tìm thấy status",
                        data = null
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