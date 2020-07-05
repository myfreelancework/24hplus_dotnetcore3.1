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

namespace _24hplusdotnetcore.Services.CRM
{
    public class CRMServices
    {
        private readonly ILogger<CRMServices> _logger;
        private readonly CustomerServices _customerServices;
        private readonly DataCRMProcessingServices _dataCRMProcessingServices;
        private readonly DataProcessingService _dataProcessingService;

        public CRMServices(ILogger<CRMServices> logger,
            CustomerServices customerServices,
            DataCRMProcessingServices dataCRMProcessingServices,
            DataProcessingService dataProcessingService)
        {
            _logger = logger;
            _customerServices = customerServices;
            _dataCRMProcessingServices = dataCRMProcessingServices;
            _dataProcessingService = dataProcessingService;
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
                        var customerCreation = new Customer
                        {
                            Personal = new Personal
                            {
                                Name = record.Potentialname,
                                Gender = record.Cf1026,
                                Phone = record.Cf854,
                                IdCard = record.Cf1050,
                                Email = record.Cf1028,
                                CurrentAddress = new Address
                                {
                                    FullAddress = record.Cf892
                                },
                                DateOfBirth = record.Cf948,
                                MaritalStatus = record.Cf1030,
                                PermanentAddress = new Address { FullAddress = record.Cf1002 },
                                PotentialNo = record.PotentialNo
                            },
                            Working = new Working
                            {
                                Job = record.Cf1246,
                                Income = record.Cf884,
                                CompanyName = record.Cf962,
                                CompanyAddress = new Address { FullAddress = record.Cf1020 },
                                CompanyPhone = record.Cf976,
                                Position = record.Cf982,
                                WorkPeriod = record.Cf984,
                                TypeOfContract = record.Cf986,
                                HealthCardInssurance = record.Cf988,

                            },
                            TemporaryAddress = new Address
                            {
                                Province = record.Cf1020
                            },
                            Loan = new Loan
                            {
                                Amount = record.Cf968,
                                Product = record.Cf1032,
                                RequestDocuments = record.Cf1036,
                                Term = record.Cf990,
                                GenarateToLead = record.Createdtime,
                                FollowedDate = record.Modifiedtime,
                            },
                            Result = new Models.Result
                            {
                                Note = record.Description,
                                Status = record.SalesStage
                            },
                            UserName = record.Modifiedby.Label.Split("-")[0],
                            Status = customerStatus,
                            Counsel = new Counsel
                            {
                                LastCounselling = record.Cf1266,
                                ApptSchedule = record.Cf1052,
                                TeleSalesCode = record.AssignedUserId?.Value,
                                Name = record.AssignedUserId?.Label,
                                Campain = record.AssignedUserId?.Value,
                                Remark = record.Cf1196,
                                Occupation = record.Cf1246
                            },
                            CRMId = record.Id
                        };
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
                    if (lstCustomer.Count > 0)
                    {
                        foreach (var item in lstCustomer)
                        {
                            var customer = _customerServices.GetCustomer(item.CustomerId);
                            var dataCRM = new Record
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
                request.AddParameter("record", "13x55730");
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                dataCRMProcessing.Status = Common.DataCRMProcessingStatus.Done;
                _dataCRMProcessingServices.UpdateByCustomerId(dataCRMProcessing.CustomerId, Common.DataCRMProcessingStatus.Done);
                _logger.LogInformation("User was pushed to CRM: {0} - Status: {1}", dataCRMProcessing.CustomerId, Common.DataCRMProcessingStatus.Done);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

            }
        }

    }
}
