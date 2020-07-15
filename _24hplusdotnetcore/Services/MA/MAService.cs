using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.CRM;
using _24hplusdotnetcore.Services.CRM;
using _24hplusdotnetcore.Settings;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MA
{
    public class MAService
    {
        private readonly ILogger<MAService> _logger;
        private readonly DataProcessingService _dataProcessingService;
        private readonly MAConfig _mAConfig;
        private readonly IRestMAService _restMAService;
        private readonly IMapper _mapper;
        private readonly LeadCrmService _leadCrmService;
        private readonly DataCRMProcessingServices _dataCRMProcessingServices;

        public MAService(
            ILogger<MAService> logger,
            DataProcessingService dataProcessingService,
            IOptions<MAConfig> mAConfig,
            IRestMAService restMAService,
            IMapper mapper,
            LeadCrmService leadCrmService,
            DataCRMProcessingServices dataCRMProcessingServices)
        {
            _logger = logger;
            _dataProcessingService = dataProcessingService;
            _mAConfig = mAConfig.Value;
            _restMAService = restMAService;
            _mapper = mapper;
            _leadCrmService = leadCrmService;
            _dataCRMProcessingServices = dataCRMProcessingServices;
        }

        public async Task PublishAsync()
        {
            try
            {
                IEnumerable<DataProcessing> dataProcessings = await _dataProcessingService.GetListAsync(
                    DataProcessingType.PUSH_LEAD_CRM_TO_MA, DataProcessingStatus.IN_PROGRESS);

                if (dataProcessings?.Any() != true)
                {
                    return;
                }

                var leadCrmIds = dataProcessings.Select(x => x.LeadCrmId);
                var leadCrms = _leadCrmService.GetByIds(leadCrmIds);

                if (leadCrms.Any() != true)
                {
                    return;
                }

                var dataProcessingUpdations = new List<DataProcessing>();

                foreach (var leadCrm in leadCrms)
                {
                    var requestData = _mapper.Map<MARequestDataModel>(leadCrm);
                    requestData.REQUEST_ID = _mAConfig.RequestId;
                    requestData.REQUEST_DOCUMENT = requestData.REQUEST_DOCUMENT.Replace(" |##| ", ",");

                    var request = new MARequestModel
                    {
                        PublicKey = _mAConfig.PublishKey,
                        Data = requestData
                    };

                    var result = await _restMAService.PushCustomerAsync(request);

                    dataProcessingUpdations.AddRange(dataProcessings.Where(x => x.LeadCrmId == leadCrm.Id));

                    if(result.Result == false)
                    {
                        await UpdateErrorLeadCrmAsync(leadCrm, LeadCrmStatus.Cancel);
                    }
                }

                foreach (var dataProcessing in dataProcessingUpdations)
                {
                    dataProcessing.Status = DataProcessingStatus.DONE;
                    dataProcessing.FinishDate = DateTime.UtcNow;
                    await _dataProcessingService.ReplaceOneAsync(dataProcessing);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private async Task UpdateErrorLeadCrmAsync(LeadCrm leadCrm, LeadCrmStatus leadCrmStatus)
        {
            leadCrm.SetCrmStatus(leadCrmStatus);
            await _leadCrmService.ReplaceOneAsync(leadCrm);

            _dataCRMProcessingServices.CreateOne(new DataCRMProcessing
            {
                LeadCrmId = leadCrm.Id,
                LeadSource = LeadSourceType.MA.ToString()
            });
        }
    }
}
