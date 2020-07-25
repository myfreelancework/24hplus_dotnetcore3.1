using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models.MC;
using _24hplusdotnetcore.Services.MC;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class CheckInfoServices
    {
        private readonly ILogger<CheckInfoServices> _logger;
        private readonly MCService _mcServices;
        private readonly MCCheckCICService _mcCheckCICService;
        private readonly IRestMCService _restMCService;
        private readonly IMapper _mapper;

        public CheckInfoServices(
            ILogger<CheckInfoServices> logger,
            MCCheckCICService mcCheckCICService,
            MCService mcServices,
            IRestMCService restMCService,
            IMapper mapper)
        {
            _logger = logger;
            _mcServices = mcServices;
            _mcCheckCICService = mcCheckCICService;
            _restMCService = restMCService;
            _mapper = mapper;
        }
        
        public async Task<MCCheckCICInfoResponseDto> CheckInfoByTypeAsync(string greentype, string citizenID, string customerName)
        {
            try
            {
                if (greentype.ToUpper() == "C")
                {
                    return await CheckInforFromMCAsync(citizenID, customerName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        
        public async Task<MCResponseDto> CheckDuplicateByTypeAsync(string greentype, string citizenID)
        {
            try
            {
                if (greentype.ToUpper() == "C")
                {
                    return await CheckCitizendAsync(citizenID);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private async Task<MCResponseDto> CheckCitizendAsync(string citizenID)
        {
            try
            {
                MCResponseDto result = await _restMCService.CheckCitizendAsync(citizenID);
                return result;
            }
            catch (Refit.ApiException ex)
            {
                _logger.LogError(ex, ex.Content);
                var error = await ex.GetContentAsAsync<MCResponseDto>();
                throw new ArgumentException(error.ReturnMes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task<MCCheckCICInfoResponseDto> CheckInforFromMCAsync(string citizenID, string customerName)
        {
            try
            {
                IEnumerable<MCCheckCICInfoResponseDto> mCCheckCICInfos = await _restMCService.CheckCICInnfoAsync(citizenID, customerName);

                var mCCheckCICInfo = mCCheckCICInfos.FirstOrDefault();
                if (mCCheckCICInfo == null)
                {
                    return null;
                }

                MCCheckCICModel cic = _mapper.Map<MCCheckCICModel>(mCCheckCICInfo);
                if (mCCheckCICInfo.Status == "NEW")
                {
                    _mcCheckCICService.CreateOne(cic);
                }
                else if (mCCheckCICInfo.Status == "CHECKING")
                {
                    var oldCic = _mcCheckCICService.FindOneByIdentity(mCCheckCICInfo.Identifier);
                    if (oldCic == null)
                    {
                        _mcCheckCICService.CreateOne(cic);
                    }
                }

                var a = _mcCheckCICService.FindOneByIdentity(mCCheckCICInfo.Identifier);

                return mCCheckCICInfo;
            }
            catch (Refit.ApiException ex)
            {
                _logger.LogError(ex, ex.Content);
                var error = await ex.GetContentAsAsync<MCResponseDto>();
                throw new ArgumentException(error.ReturnMes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        
        public async Task<MCCheckCatResponseDto> CheckCatAsync(string GreenType, string companyTaxNumber)
        {
            try
            {
                if (GreenType.ToUpper() == Common.GeenType.GreenC)
                {
                    return await _mcServices.CheckCatAsync(companyTaxNumber);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
