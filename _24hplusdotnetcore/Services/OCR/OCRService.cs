using _24hplusdotnetcore.ModelDtos;
using Microsoft.Extensions.Logging;
using Refit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.OCR
{
    public interface IOCRService
    {
        Task<OCRTranferResponse> TransferAsync(OCRTranferRequest oCRTranferRequest);
        Task<OCRReceiveResponse> ReceiveAsync(string keyImages);
    }

    public class OCRService: IOCRService
    {
        private readonly ILogger<OCRService> _logger;
        private readonly IRestOCRService _restOCRService;

        public OCRService(ILogger<OCRService> logger, IRestOCRService restOCRService)
        {
            _logger = logger;
            _restOCRService = restOCRService;
        }

        public async Task<OCRTranferResponse> TransferAsync(OCRTranferRequest oCRTranferRequest)
        {
            try
            {
                var files = oCRTranferRequest.Files
                .Where(file => file.Length > 0)
                .Select(file =>
                {
                    var stream = file.OpenReadStream();
                    var streamPart = new StreamPart(stream, file.FileName, file.ContentType);
                    return streamPart;
                });

                var result = await _restOCRService.TransferAsync(oCRTranferRequest.Type, files);

                return result;
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<OCRReceiveResponse> ReceiveAsync(string keyImages)
        {
            try
            {
                var result = await _restOCRService.ReceiveAsync(keyImages);

                return result;
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
