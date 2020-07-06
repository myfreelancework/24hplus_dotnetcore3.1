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

namespace _24hplusdotnetcore.Services.CRM
{
    public class CRMServices
    {
        private readonly ILogger<CRMServices> _logger;
        private readonly CustomerServices _customerServices;
        private readonly DataCRMProcessingServices _dataCRMProcessingServices;
        private readonly DataProcessingService _dataProcessingService;
        private readonly IMapper _mapper;

        public CRMServices(ILogger<CRMServices> logger,
            CustomerServices customerServices,
            DataCRMProcessingServices dataCRMProcessingServices,
            DataProcessingService dataProcessingService,
            IMapper mapper)
        {
            _logger = logger;
            _customerServices = customerServices;
            _dataCRMProcessingServices = dataCRMProcessingServices;
            _dataProcessingService = dataProcessingService;
            _mapper = mapper;
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

        private long AddNewCustomerFromCRM(CrmCustomerData crmCustomer, string customerStatus, string dataProcessingType)
        {
            try
            {
                if (crmCustomer?.Result?.Records?.Any() != true)
                {
                    return 0;
                }

                var idCards = crmCustomer.Result.Records.Select(x => x.Cf1050);
                IEnumerable<Customer> customers = _customerServices.GetByIdCards(idCards);

                var customerCreations = new List<Customer>();
                var dataProcessingCreations = new List<DataProcessing>();

                foreach (var record in crmCustomer.Result.Records)
                {
                    var customer = customers?.FirstOrDefault(x => string.Equals(x.Personal.IdCard, record.Cf1050, StringComparison.OrdinalIgnoreCase));
                    if (customer == null)
                    {
                        var customerCreation = _mapper.Map<Customer>(record);
                        customerCreation.Status = customerStatus;
                        customerCreations.Add(customerCreation);
                    }
                    else
                    {
                        dataProcessingCreations.Add(new DataProcessing
                        {
                            CustomerId = customer.Id,
                            DataProcessingType = dataProcessingType
                        });
                    }
                }

                if (customerCreations.Any())
                {
                    _customerServices.InsertMany(customerCreations);

                    dataProcessingCreations.AddRange(customerCreations.Select(customer => new DataProcessing
                    {
                        CustomerId = customer.Id,
                        DataProcessingType = dataProcessingType
                    }));

                    _dataCRMProcessingServices.InsertMany(customerCreations.Select(customer => new DataCRMProcessing { 
                        CustomerId = customer.Id,
                        Status = DataCRMProcessingStatus.InProgress, 
                        LeadSource = LeadSourceType.MA.ToString() 
                    }));
                }

                if (dataProcessingCreations.Any())
                {
                    _dataProcessingService.InsertMany(dataProcessingCreations);
                }


                return dataProcessingCreations.Count;
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

        public long GetCustomerFromCRM(string query)
        {
            try
            {
                string session = CRMLogin();
                if (string.IsNullOrEmpty(session))
                {
                    return 0;
                }

                CrmCustomerData dataFromCRM = QueryLead(session, query);
                return AddNewCustomerFromCRM(dataFromCRM, CustomerStatus.PUSH_MA, DataProcessingType.PUSH_CUSTOMER_CRM_TO_MA);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return -1;
            }
        }

        public void AddingDataToCRM()
        {
            try
            {
                string session = CRMLogin();
                if (!string.IsNullOrEmpty(session))
                {
                    var lstCustomer = _dataCRMProcessingServices.GetDataCRMProcessings(Common.DataCRMProcessingStatus.InProgress);
                    if (lstCustomer.Any())
                    {
                        foreach (var item in lstCustomer)
                        {
                            var customer = _customerServices.GetCustomer(item.CustomerId);
                            Record dataCRM;

                            if(string.Equals(item.LeadSource, LeadSourceType.MA.ToString()))
                            {
                                dataCRM = _mapper.Map<Record>(customer);
                                dataCRM.Cf1178 = LeadSourceType.MA.ToString();
                                dataCRM.Leadsource = LeadSourceType.MA.ToString();
                            }
                            else
                            {
                                dataCRM = new Record
                                {
                                    Cf1178 = "MC",
                                    Potentialname = customer.Personal.Name,
                                    Cf1026 = customer.Personal.Gender,
                                    Leadsource = "MC",
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
                            }

                            PushDataToCRM(dataCRM, session, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
            }
        }

        private void PushDataToCRM(Record dataCRM, string session, DataCRMProcessing dataCRMProcessing)
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
