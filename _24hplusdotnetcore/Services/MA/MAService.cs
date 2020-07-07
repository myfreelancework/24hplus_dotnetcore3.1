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
        private readonly CustomerServices _customerServices;
        private readonly MAConfig _mAConfig;
        private readonly IRestMAService _restMAService;
        private readonly IMapper _mapper;

        public MAService(
            ILogger<MAService> logger,
            DataProcessingService dataProcessingService,
            CustomerServices customerServices,
            IOptions<MAConfig> mAConfig,
            IRestMAService restMAService,
            IMapper mapper)
        {
            _logger = logger;
            _dataProcessingService = dataProcessingService;
            _customerServices = customerServices;
            _mAConfig = mAConfig.Value;
            _restMAService = restMAService;
            _mapper = mapper;
        }

        public async Task PublishAsync()
        {
            try
            {
                IEnumerable<DataProcessing> dataProcessings = _dataProcessingService.GetList(DataProcessingType.PUSH_CUSTOMER_CRM_TO_MA, DataProcessingStatus.IN_PROGRESS);
                if (dataProcessings?.Any() != true)
                {
                    return;
                }

                var customerIds = dataProcessings.Select(x => x.CustomerId);
                IEnumerable<Customer> customers = _customerServices.GetByIds(customerIds);
                if (customers?.Any() != true)
                {
                    return;
                }

                var dataProcessingIds = new List<string>();

                foreach (var customer in customers)
                {
                    var requestData = _mapper.Map<MARequestDataModel>(customer);
                    requestData.REQUEST_ID = _mAConfig.RequestId;

                    var request = new MARequestModel
                    {
                        PublicKey = _mAConfig.PublishKey,
                        Data = requestData
                    };

                    var result = await _restMAService.PushCustomerAsync(request);

                    if (result.Result == true)
                    {
                        var dataProcessing = dataProcessings.First(x => x.CustomerId == customer.Id);
                        dataProcessingIds.Add(dataProcessing.Id);
                    }
                }

                if (dataProcessingIds.Any())
                {
                    _dataProcessingService.DeleteByIds(dataProcessingIds);
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
