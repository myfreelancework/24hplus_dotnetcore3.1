using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Settings;
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

        public MAService(
            ILogger<MAService> logger,
            DataProcessingService dataProcessingService,
            CustomerServices customerServices,
            IOptions<MAConfig> mAConfig,
            IRestMAService restMAService)
        {
            _logger = logger;
            _dataProcessingService = dataProcessingService;
            _customerServices = customerServices;
            _mAConfig = mAConfig.Value;
            _restMAService = restMAService;
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
                    MARequestModel request = MappingCustomerToMA(customer);

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

        private MARequestModel MappingCustomerToMA(Customer customer)
        {
            return new MARequestModel
            {
                PublicKey = _mAConfig.PublishKey,
                Data = new MARequestDataModel
                {
                    REQUEST_ID = _mAConfig.RequestId,
                    LEAD_ID = customer.Personal?.PotentialNo,
                    ID_NO = customer.Personal?.IdCard,
                    CONTACT_NAME = customer.Personal?.Name,
                    PHONE = customer.Personal?.Phone,
                    CURRENT_ADDRESS = customer.Personal?.CurrentAddress?.FullAddress,
                    PRODUCT = customer.Loan?.Product,
                    T_STATUS_DATE = customer.Counsel?.LastCounselling,
                    APPOINTMENT_DATE = customer.Counsel?.ApptSchedule,
                    APPOINTMENT_ADDRESS = customer.Personal?.CurrentAddress?.FullAddress,
                    TSA_IN_CHARGE = customer.Counsel?.TeleSalesCode,
                    TST_TEAM = customer.Counsel?.TeamCode,
                    REQUEST_DOCUMENT = customer.Loan?.RequestDocuments,
                    DOB = customer.Personal?.DateOfBirth,
                    GENDER = customer.Personal?.Gender,
                    COMPANY_NAME = customer.Working?.CompanyName,
                    COMPANY_ADDR = customer.Working?.CompanyAddress?.FullAddress,
                    TEL_COMPANY = customer.Working?.CompanyPhone,
                    AREA = customer.TemporaryAddress?.FullAddress,
                    MARITAL_STATUS = customer.Personal?.MaritalStatus,
                    OWNER = customer.Loan?.Owner,
                    INCOME = customer.Working?.Income,
                    POSITION = customer.Working?.Position,
                    WORK_PERIOD = customer.Working?.WorkPeriod,
                    TYPE_OF_CONTRACT = customer.Working?.TypeOfContract,
                    HEALTH_CARD = customer.Working?.HealthCardInssurance,
                    LOAN_AMOUNT = customer.Loan?.Amount,
                    TENURE = customer.Loan?.Term,
                    APP_DATE = customer.Loan?.AppDate,
                    DISBURSAL_DATE = customer.Loan?.DisbursalDate,
                    GENERATE_TO_LEAD = customer.Loan?.GenarateToLead,
                    FOLLOWED_DATE = customer.Loan?.FollowedDate,
                    PERMANENT_ADDR = customer.Personal?.PermanentAddress?.FullAddress,
                    TSA_NAME = customer.Counsel?.Name,
                    TSA_CAMPAIN = customer.Counsel?.Campain,
                    TSA_GROUP = customer.Counsel?.GroupCode,
                    TSA_LAST_NOTES = customer.Counsel?.Remark,
                    OCCUPATION = customer.Counsel?.Occupation,
                    ROUTE = customer.Route
                }
            };
        }
    }
}
