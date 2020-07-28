using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.ModelDtos;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.OCR
{
    public interface IRestOCRService
    {
        [Multipart]
        [Post("/OCRSystem/api/Transfer")]
        Task<OCRTranferResponse> TransferAsync(OCRType type, [AliasAs("files")] IEnumerable<StreamPart> streams);

        [Get("/OCRSystem/api/Receive/{keyImages}")]
        Task<OCRReceiveResponse> ReceiveAsync(string keyImages);
    }
}
