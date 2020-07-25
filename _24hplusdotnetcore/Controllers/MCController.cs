using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Attributes;
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using _24hplusdotnetcore.Services;
using _24hplusdotnetcore.Services.MC;
using _24hplusdotnetcore.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly CustomerServices _customerService;
        private readonly MCNotificationService _mcNotificationService;
        private readonly MCCheckCICService _mcCheckCICService;
        private readonly IMapper _mapper;
        private readonly MCConfig _mCConfig;

        public MCController(ILogger<MCController> logger,
        MCService mcService,
        CustomerServices customerService,
        MCCheckCICService mcCheckCICService,
        MCNotificationService mcNotificationService,
        IMapper mapper,
        IOptions<MCConfig> mCConfigOption)
        {
            _logger = logger;
            _mcService = mcService;
            _mcCheckCICService = mcCheckCICService;
            _mcNotificationService = mcNotificationService;
            _customerService = customerService;
            _mapper = mapper;
            _mCConfig = mCConfigOption.Value;
        }

        [HttpGet]
        [Route("api/getmcproduct")]
        public async Task<ActionResult<ResponseContext>> GetMCProductAsync()
        {
            try
            {
                IEnumerable<MCProduct> mCProducts = await _mcService.GetProductAsync();
                return Ok(new ResponseContext
                {
                    code = (int)ResponseCode.SUCCESS,
                    message = Message.SUCCESS,
                    data = mCProducts
                });
            }
            catch (Exception ex)
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


        [MCAuthorize]
        [HttpPost("api/mc/notification")]
        public async Task<ActionResult<ResponseContext>> PushNotification(MCNotificationDto noti)
        {
            try
            {
                _mcNotificationService.CreateOne(noti);
                var customer = _customerService.GetByMCId(noti.Id);
                if (customer != null)
                {
                    var dto = new CustomerUpdateStatusDto();
                    dto.LeadSource = LeadSourceType.MC.ToString();
                    dto.CustomerId = customer.Id;
                    dto.Reason = noti.CurrentStatus;
                    if (MCNotificationMessage.Return.Where(x => x == noti.CurrentStatus).Any())
                    {
                        dto.Status = CustomerStatus.RETURN;
                        // Get return checklist
                        CustomerCheckListResponseModel returnChecklist = await _mcService.GetReturnCheckListAsync(customer.Id);
                        if (returnChecklist != null)
                        {
                            var returnDocuments = _mapper.Map<IEnumerable<GroupDocument>>(returnChecklist.CheckList);
                            _customerService.UpdateCustomerMCReturnDocuments(customer.Id, returnDocuments);
                        }
                        // get reason return
                        GetCaseRequestDto getCaseRequestDto = new GetCaseRequestDto();
                        getCaseRequestDto.Status = CaseStatus.ABORT;
                        getCaseRequestDto.Keyword = customer.Personal.Name;
                        getCaseRequestDto.SaleCode = _mCConfig.SaleCode;
                        IEnumerable<GetCaseMCResponseDto> cases = await _mcService.GetCasesAsync(getCaseRequestDto);
                        if (cases.Any())
                        {
                            var fistCase = cases.First();
                            if (fistCase.Id == customer.MCId && fistCase.Reasons.Any())
                            {
                                dto.Reason = fistCase.Reasons.First().Reason + ", " + fistCase.Reasons.First().ReasonDetail;
                            }
                        }
                    }
                    else if (MCNotificationMessage.Cancel.Where(x => x == noti.CurrentStatus).Any())
                    {
                        dto.Status = CustomerStatus.CANCEL;
                    }
                    else if (MCNotificationMessage.Succes == noti.CurrentStatus)
                    {
                        dto.Status = CustomerStatus.SUCCESS;
                    }
                    else
                    {
                        dto.Status = CustomerStatus.PROCESSING;
                    }
                    if (customer.MCAppnumber == 0)
                    {
                        _customerService.UpdateMCAppId(noti);
                    }
                    _customerService.UpdateStatus(dto);
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
                    ReturnMes = ex.Message
                });
            }
        }

        [MCAuthorize]
        [HttpPost("api/mc/update-cic")]
        public async Task<ActionResult<ResponseContext>> UpdateCICAsync(MCUpdateCICDto dto)
        {
            try
            {
                var oldCic = _mcCheckCICService.FindOneByIdentity(dto.Identifier);
                if (oldCic != null)
                {
                    oldCic.RequestId = dto.RequestId;
                    oldCic.CicResult = dto.CicResult;
                    oldCic.Description = dto.Description;
                    oldCic.CicImageLink = dto.CicImageLink;
                    oldCic.LastUpdateTime = dto.LastUpdateTime;
                    oldCic.Status = dto.Status;
                    await _mcCheckCICService.ReplaceOneAsync(oldCic);
                }
                else
                {
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

        [AllowAnonymous]
        [HttpGet]
        [Route("api/mc/push-mc")]
        public async Task<ActionResult<ResponseContext>> CheckListAsync()
        {
            try
            {
                await _mcService.PushDataToMCAsync();
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

        [HttpPost("api/mc/cancel-case")]
        public async Task<ActionResult<ResponseContext>> CancelCaseAsync(CancelCaseRequestDto cancelCaseRequestDto)
        {
            try
            {
                MCSuccessResponseDto mCSuccessResponseDto = await _mcService.CancelCaseAsync(cancelCaseRequestDto);

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = mCSuccessResponseDto
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

        /// <summary>
        /// Get list case note
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("api/mc/case-note/{customerId}")]
        public async Task<ActionResult<ResponseContext>> GetCaseNoteAsync(string customerId)
        {
            try
            {
                MCCaseNoteListDto mCCaseNoteListDto = await _mcService.GetCaseNoteAsync(customerId);

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = mCCaseNoteListDto.MCNotesEntries.MCNotesEntry.Last()
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

        [HttpPost("api/mc/send-case-note")]
        public async Task<ActionResult<ResponseContext>> SendCaseNoteAsync(SendCaseNoteRequestDto sendCaseNoteRequestDto)
        {
            try
            {
                MCSuccessResponseDto mCSuccessResponseDto = await _mcService.SendCaseNoteAsync(sendCaseNoteRequestDto);

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = mCSuccessResponseDto
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

        [HttpGet("api/mc/return-check-list")]
        public async Task<ActionResult<ResponseContext>> GetReturnCheckListAsync(string customerId)
        {
            try
            {
                CustomerCheckListResponseModel customerCheckListResponseModel = await _mcService.GetReturnCheckListAsync(customerId);

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = customerCheckListResponseModel
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

        [HttpGet("api/mc/cases")]
        public async Task<ActionResult<ResponseContext>> GetCasesAsync([FromQuery] GetCaseRequestDto getCaseRequestDto)
        {
            try
            {
                IEnumerable<GetCaseMCResponseDto> cases = await _mcService.GetCasesAsync(getCaseRequestDto);

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = cases
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