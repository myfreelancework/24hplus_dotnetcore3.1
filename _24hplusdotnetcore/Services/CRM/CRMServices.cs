using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using _24hplusdotnetcore.Models.CRM;
using System.Linq;
using _24hplusdotnetcore.Models;
using MongoDB.Bson;
using _24hplusdotnetcore.Common;
using AutoMapper;
using _24hplusdotnetcore.Common.Enums;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.CRM
{
    public class CRMServices
    {
        private readonly ILogger<CRMServices> _logger;
        private readonly CustomerServices _customerServices;
        private readonly DataCRMProcessingServices _dataCRMProcessingServices;
        private readonly DataProcessingService _dataProcessingService;
        private readonly IMapper _mapper;
        private readonly LeadCrmService _leadCrmService;

        public CRMServices(ILogger<CRMServices> logger,
            CustomerServices customerServices,
            DataCRMProcessingServices dataCRMProcessingServices,
            DataProcessingService dataProcessingService,
            IMapper mapper,
            LeadCrmService leadCrmService)
        {
            _logger = logger;
            _customerServices = customerServices;
            _dataCRMProcessingServices = dataCRMProcessingServices;
            _dataProcessingService = dataProcessingService;
            _mapper = mapper;
            _leadCrmService = leadCrmService;
        }

        private string CRMLogin()
        {
            string session = string.Empty;
            try
            {
                var client = new RestClient(Common.Constants.Url.CRMLogin);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("_operation", "login");
                request.AddParameter("username", "" + Common.Constants.ConfigRequest.CRM_UserName + "");
                request.AddParameter("password", "" + Common.Constants.ConfigRequest.CRM_Password + "");
                IRestResponse response = client.Execute(request);
                dynamic context = JsonConvert.DeserializeObject<dynamic>(response.Content);
                session = context.result.login.session;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return session;
        }

        private CrmCustomerData QueryLead(string session, string queryString)
        {
            try
            {
                var client = new RestClient(Common.Constants.Url.CRMQueryLead);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("_operation", "query");
                request.AddParameter("_session", "" + session + "");
                request.AddParameter("query", "" + queryString + "");
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<CrmCustomerData>(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private long AddNewCustomerFromCRM(CrmCustomerData crmCustomer)
        {
            long insertCount = 0;
            try
            {

                var arrCustomer = crmCustomer.Result.Records;

                if (arrCustomer.Length > 0)
                {
                    foreach (var item in arrCustomer)
                    {
                        if (_customerServices.GetCustomerByIdCard(item.Cf1050) == null)
                        {
                            Customer objCustomer = new Customer();
                            objCustomer.Personal = new Personal();
                            objCustomer.Personal.Name = item.Potentialname;
                            objCustomer.Personal.Gender = item.Cf1026;
                            objCustomer.Personal.Phone = item.Cf854;
                            objCustomer.Personal.IdCard = item.Cf1050;
                            objCustomer.Personal.Email = item.Cf1028;
                            objCustomer.Working = new Working();
                            objCustomer.Working.Job = item.Cf1246;
                            objCustomer.Working.Income = item.Cf884;
                            objCustomer.TemporaryAddress = new Address();
                            objCustomer.TemporaryAddress.Province = item.Cf1020;
                            objCustomer.Loan = new Loan();
                            objCustomer.Loan.Amount = item.Cf968;
                            objCustomer.Result = new Models.Result();
                            objCustomer.Result.Note = item.Description;
                            objCustomer.Result.Status = item.SalesStage;
                            objCustomer.UserName = item.Modifiedby.Label.Split("-")[0];
                            objCustomer.Status = Common.CustomerStatus.POTENTIAL;
                            var newcustomer = _customerServices.CreateCustomer(objCustomer);
                            if (newcustomer != null)
                            {
                                insertCount++;
                            }
                        }

                    }
                }
                _logger.LogInformation("Number of customer from CRM: " + insertCount);
                return insertCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return -1;
            }
        }

        public long GetCustomerFromCRM()
        {
            try
            {
                string session = CRMLogin();
                if (!string.IsNullOrEmpty(session))
                {
                    dynamic dataFromCRM = QueryLead(session, Common.Constants.ConfigRequest.CRM_Query);
                    return AddNewCustomerFromCRM(dataFromCRM);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return -1;
            }
        }

        public async Task UpsertLeadCrmAsync(CrmCustomerData crmCustomer, string dataProcessingType)
        {
            try
            {
                if (crmCustomer?.Result?.Records?.Any() != true)
                {
                    return;
                }

                IEnumerable<string> leadCrmIds = crmCustomer.Result.Records.Select(x => x.Id);
                IEnumerable<LeadCrm> leadCrms = await _leadCrmService.GetByLeadCrmIdsAsync(leadCrmIds);

                var dataProcessingCreations = new List<DataProcessing>();
                var dataCRMProcessingCreations = new List<DataCRMProcessing>();

                foreach (Record record in crmCustomer.Result.Records)
                {
                    LeadCrm leadCrm = leadCrms.FirstOrDefault(x => x.LeadCrmId == record.Id);
                    if (leadCrm == null)
                    {
                        leadCrm = _mapper.Map<LeadCrm>(record);
                        await _leadCrmService.InsertAsync(leadCrm);
                    }
                    else
                    {
                        _mapper.Map(record, leadCrm);
                        await _leadCrmService.ReplaceOneAsync(leadCrm);
                    }

                    dataProcessingCreations.Add(new DataProcessing
                    {
                        LeadCrmId = leadCrm.Id,
                        DataProcessingType = dataProcessingType
                    });
                    dataCRMProcessingCreations.Add(new DataCRMProcessing
                    {

                        LeadCrmId = leadCrm.Id,
                        LeadSource = LeadSourceType.MA.ToString()
                    });
                }

                _dataProcessingService.InsertMany(dataProcessingCreations);
                _dataCRMProcessingServices.InsertMany(dataCRMProcessingCreations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task GetCustomerFromCrmAsync(string query)
        {
            try
            {
                string session = CRMLogin();
                if (string.IsNullOrEmpty(session))
                {
                    throw new Exception("Token is null or empty");
                }
                CrmCustomerData dataFromCRM = QueryLead(session, query);
                await UpsertLeadCrmAsync(dataFromCRM, DataProcessingType.PUSH_LEAD_CRM_TO_MA);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void AddingDataToCRM()
        {
            try
            {
                string session = CRMLogin();
                if (!string.IsNullOrEmpty(session))
                {
                    var dataCRMProcessings = _dataCRMProcessingServices.GetDataCRMProcessings(Common.DataCRMProcessingStatus.InProgress);
                    if (!dataCRMProcessings.Any())
                    {
                        return;
                    }

                    IEnumerable<string> customerIds = dataCRMProcessings
                        .Where(x => !string.IsNullOrEmpty(x.CustomerId))
                        .Select(x => x.CustomerId);
                    IEnumerable<Customer> customers = _customerServices.GetByIds(customerIds);

                    IEnumerable<string> leadCrmIds = dataCRMProcessings
                        .Where(x => !string.IsNullOrEmpty(x.LeadCrmId))
                        .Select(x => x.LeadCrmId);
                    IEnumerable<LeadCrm> leadCrms = _leadCrmService.GetByIds(leadCrmIds);

                    foreach (var dataCRMProcessing in dataCRMProcessings)
                    {

                        if (string.Equals(dataCRMProcessing.LeadSource, LeadSourceType.MA.ToString()))
                        {
                            LeadCrm leadCrm = leadCrms.FirstOrDefault(x => x.Id == dataCRMProcessing.LeadCrmId);
                            if (leadCrm != null)
                            {
                                var dataCRM = _mapper.Map<CRMRequestDto>(leadCrm);
                                dataCRM.Cf1178 = "MIRAE ASSET";
                                dataCRM.Leadsource = "Telesales 24hPlus -2020";
                                dataCRM.Cf1206 = "1";
                                PushDataToCRM(dataCRM, session, dataCRMProcessing);
                            }
                        }
                        else
                        {
                            Customer customer = customers.FirstOrDefault(x => x.Id == dataCRMProcessing.CustomerId);
                            if (customer != null)
                            {
                                Record dataCRM = new Record
                                {
                                    Cf1178 = "MC",
                                    Potentialname = customer.Personal.Name,
                                    Cf1026 = customer.Personal.Gender,
                                    Leadsource = "MobileGreenC",
                                    Cf854 = customer.Personal.Phone,
                                    Cf1050 = customer.Personal.IdCard,
                                    Cf1028 = customer.Working.Job,
                                    Cf884 = customer.Working.Income,
                                    Cf1020 = customer.ResidentAddress.Province,
                                    Cf1032 = customer.Loan.Category,
                                    Cf1040 = customer.Loan.Product,
                                    Cf968 = customer.Loan.Amount,
                                    Cf990 = customer.Loan.Term,
                                    Cf1052 = "-",
                                    Cf1054 = customer.Loan.SignAddress,
                                    Cf1036 = "CHỨNG MINH NHÂN DÂN |##| HỘ KHẨU",
                                    SalesStage = "1.KH mới",
                                    Cf1184 = "-",
                                    Cf1188 = "-",
                                    AssignedUserId = new AssignedUserId
                                    {
                                        Value = "19x2335"
                                    },
                                    Cf1244 = "AS",
                                    Cf1256 = "-",
                                    Cf1264 = "????",
                                    Cf1230 = ""
                                };
                                PushDataToCRM(dataCRM, session, dataCRMProcessing);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
            }
        }

        private void PushDataToCRM(CRMBaseModel dataCRM, string session, DataCRMProcessing dataCRMProcessing)
        {
            try
            {
                var client = new RestClient(Common.Constants.Url.CRMURL);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddParameter("_operation", "saveRecord");
                request.AddParameter("values", "" + JsonConvert.SerializeObject(dataCRM) + "");
                request.AddParameter("_session", "" + session + "");
                request.AddParameter("module", "Potentials");
                //request.AddParameter("record", "13x55730");
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                dataCRMProcessing.Status = Common.DataCRMProcessingStatus.Done;
                dataCRMProcessing.FinishDate = DateTime.UtcNow;
                _dataCRMProcessingServices.UpdateByCustomerId(dataCRMProcessing, Common.DataCRMProcessingStatus.Done);
                _logger.LogInformation("User was pushed to CRM: {0} - Status: {1}", dataCRMProcessing.CustomerId, Common.DataCRMProcessingStatus.Done);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

            }
        }

    }
}
