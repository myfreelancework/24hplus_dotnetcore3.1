using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services.OCR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    [Route("api/ocr")]
    public class OCRController: ControllerBase
    {
        private readonly ILogger<OCRController> _logger;
        private readonly IOCRService _oCRService;

        public OCRController(
            ILogger<OCRController> logger,
            IOCRService oCRService)
        {
            _logger = logger;
            _oCRService = oCRService;
        }

        [HttpPost("tranfer")]
        public async Task<IActionResult> TranferAsync([FromForm] OCRTranferRequest oCRTranferRequest)
        {
            try
            {
                OCRTranferResponse oCRTranferResponse = await _oCRService.TransferAsync(oCRTranferRequest);

                if(oCRTranferResponse.Success)
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.SUCCESS,
                        message = Common.Message.SUCCESS,
                        data = oCRTranferResponse
                    });
                }

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.ERROR,
                    message = oCRTranferResponse.Message,
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

        [HttpGet("receive/{keyImages}")]
        public async Task<IActionResult> ReceiveAsync(string keyImages)
        {
            try
            {
                OCRReceiveResponse oCRReceiveResponse = await _oCRService.ReceiveAsync(keyImages);

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = oCRReceiveResponse
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
