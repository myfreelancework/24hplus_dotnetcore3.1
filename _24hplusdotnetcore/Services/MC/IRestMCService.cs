﻿using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models.MC;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MC
{
    public interface IRestMCService
    {
        [Get("/mcMobileService/service/v1.0/mobile-4sales/check-list")]
        [Headers("Authorization: Bearer")]
        Task<CustomerCheckListResponseModel> CheckListAsync([Query] CustomerCheckListRequestModel customerCheckListRequestModel);
        
        [Get("/mcMobileService/service/v1.0/mobile-4sales/kiosks")]
        [Headers("Authorization: Bearer")]
        Task<IEnumerable<KiosModel>> GetKiosAsync();

        [Post("/mcMobileService/service/v1.0/mobile-4sales/cancel-case")]
        [Headers("Authorization: Bearer")]
        Task<MCSuccessResponseDto> CancelCaseAsync(MCCancelCaseRequestDto cancelCaseRequestDto);

        [Get("/mcMobileService/service/v1.0/mobile-4sales/list-case-note/{appNumber}")]
        [Headers("Authorization: Bearer")]
        Task<MCCaseNoteListDto> GetCaseNoteAsync(int appNumber);

        [Post("/mcMobileService/service/v1.0/mobile-4sales/send-case-note")]
        [Headers("Authorization: Bearer")]
        Task<MCSuccessResponseDto> SendCaseNoteAsync(MCSendCaseNoteRequestDto mCSendCaseNoteRequestDto);

        [Get("/mcMobileService/service/v1.0/mobile-4sales/third-party/checklist")]
        [Headers("Authorization: Bearer")]
        Task<CustomerCheckListResponseModel> GetReturnCheckListAsync(string appId);
        
        [Get("/mcMobileService/service/v1.0/mobile-4sales/cases")]
        [Headers("Authorization: Bearer")]
        Task<IEnumerable<GetCaseMCResponseDto>> GetCasesAsync([Query] GetCaseMCRequestDto getCaseMCRequestDto);

        [Get("/mcMobileService/service/v1.0/mobile-4sales/check-cat")]
        [Headers("Authorization: Bearer")]
        Task<MCCheckCatResponseDto> CheckCatAsync(string companyTaxNumber);

        [Get("/mcMobileService/service/v1.0/mobile-4sales/products")]
        [Headers("Authorization: Bearer")]
        Task<IEnumerable<MCProduct>> GetProductAsync();

        [Get("/mcMobileService/service/v1.0/mobile-4sales/check-identifier")]
        [Headers("Authorization: Bearer")]
        Task<MCResponseDto> CheckCitizendAsync(string citizenId);

        [Get("/mcMobileService/service/v1.0/mobile-4sales/check-cic/check")]
        [Headers("Authorization: Bearer")]
        Task<IEnumerable<MCCheckCICInfoResponseDto>> CheckCICInnfoAsync(string citizenID, string customerName);
    }
}
