using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
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

        public MAService(
            ILogger<MAService> logger,
            DataProcessingService dataProcessingService,
            IOptions<MAConfig> mAConfig,
            IRestMAService restMAService,
            IMapper mapper,
            LeadCrmService leadCrmService)
        {
            _logger = logger;
            _dataProcessingService = dataProcessingService;
            _mAConfig = mAConfig.Value;
            _restMAService = restMAService;
            _mapper = mapper;
            _leadCrmService = leadCrmService;
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

                    if (result.Result == true)
                    {
                        dataProcessingUpdations.AddRange(dataProcessings.Where(x => x.LeadCrmId == leadCrm.Id));

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
    }
}
