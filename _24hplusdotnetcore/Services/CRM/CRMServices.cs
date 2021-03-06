﻿using Microsoft.Extensions.Logging;
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
        private readonly ConfigServices _configServices;
        private readonly IMapper _mapper;
        private readonly LeadCrmService _leadCrmService;

        public CRMServices(ILogger<CRMServices> logger,
            CustomerServices customerServices,
            DataCRMProcessingServices dataCRMProcessingServices,
            DataProcessingService dataProcessingService,
            ConfigServices configServices,
            IMapper mapper,
            LeadCrmService leadCrmService)
        {
            _logger = logger;
            _customerServices = customerServices;
            _dataCRMProcessingServices = dataCRMProcessingServices;
            _dataProcessingService = dataProcessingService;
            _mapper = mapper;
            _leadCrmService = leadCrmService;
            _configServices = configServices;
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
                        UpSertCrmResponse crmCustomerResponse = null;

                        if (string.Equals(dataCRMProcessing.LeadSource, LeadSourceType.MA.ToString()))
                        {
                            LeadCrm leadCrm = leadCrms.FirstOrDefault(x => x.Id == dataCRMProcessing.LeadCrmId);
                            if (leadCrm != null)
                            {
                                var dataCRM = _mapper.Map<CRMRequestDto>(leadCrm);
                                dataCRM.Cf1178 = "MIRAE ASSET";
                                dataCRM.Leadsource = "Telesales 24hPlus -2020";
                                dataCRM.Cf1206 = "1";
                                crmCustomerResponse = PushDataToCRM(dataCRM, session, dataCRMProcessing);
                            }
                        }
                        else if (string.Equals(dataCRMProcessing.LeadSource, LeadSourceType.FIBO.ToString()))
                        {
                            LeadCrm leadCrm = leadCrms.FirstOrDefault(x => x.Id == dataCRMProcessing.LeadCrmId);
                            if (leadCrm != null)
                            {
                                var dataCRM = _mapper.Map<CRMRequestDto>(leadCrm);
                                dataCRM.AssignedUserId = "19x2719";
                                crmCustomerResponse = PushDataToCRM(dataCRM, session, dataCRMProcessing);
                                if (crmCustomerResponse?.Result?.Record != null)
                                {
                                    leadCrm.LeadCrmId = crmCustomerResponse.Result.Record.Id;
                                    leadCrm.PotentialNo = crmCustomerResponse.Result.Record.PotentialNo;
                                    _leadCrmService.ReplaceOne(leadCrm);
                                }
                            }
                        }
                        else
                        {
                            Customer customer = customers.FirstOrDefault(x => x.Id == dataCRMProcessing.CustomerId);
                            if (customer != null)
                            {
                                string listLinkDocuments = "";
                                string listTypeDocuments = "";
                                string residentAddress = customer.ResidentAddress.Street + ", "
                                + customer.ResidentAddress.Ward + ", "
                                + customer.ResidentAddress.District + ", "
                                + customer.ResidentAddress.Province + ", ";
                                string tempAddress = customer.TemporaryAddress.Street + ", "
                                + customer.TemporaryAddress.Ward + ", "
                                + customer.TemporaryAddress.District + ", "
                                + customer.TemporaryAddress.Province + ", ";

                                var idCarDate = customer.Personal.IdCardDate.Split("/");
                                var dob = customer.Personal.DateOfBirth.Split("/");

                                Int32.TryParse(customer.Loan.Amount.Replace(",", string.Empty), out int amount);
                                double totalAmount = amount;
                                if (customer.Loan.BuyInsurance == true)
                                {
                                    totalAmount += totalAmount * 0.055;
                                }

                                foreach (var group in customer.Documents)
                                {
                                    var groupId = group.GroupId;
                                    foreach (var doc in group.Documents)
                                    {
                                        foreach (var media in doc.UploadedMedias)
                                        {
                                            listLinkDocuments += media.Uri + "; ";
                                            listTypeDocuments += doc.DocumentName.ToUpper() + " |##| ";
                                        }
                                    }
                                }


                                CRMRequestDto dataCRM = new CRMRequestDto
                                {
                                    Cf1178 = "MCREDIT",
                                    Leadsource = "MobileGreenC",
                                    Potentialname = customer.Personal.Name,
                                    Cf1050 = customer.Personal.IdCard,
                                    Cf1350 = idCarDate[2] + "-" + idCarDate[1] + "-" + idCarDate[0],
                                    Cf1408 = customer.Personal.IdCardProvince,
                                    Cf1026 = customer.Personal.Gender,
                                    Cf948 = dob[2] + "-" + dob[1] + "-" + dob[0],
                                    Cf854 = customer.Personal.Phone,
                                    // @todo
                                    Cf1002 = residentAddress,
                                    Cf1238 = customer.IsTheSameResidentAddress == true ? "1" : "0",
                                    Cf892 = customer.IsTheSameResidentAddress == true ? residentAddress : tempAddress,
                                    Cf1246 = customer.Working.Job,
                                    Cf964 = customer.Working.CompanyAddress.Street,
                                    Cf884 = customer.Working.Income.Replace(",", string.Empty),
                                    // Cf1020 = customer.ResidentAddress.Province,
                                    Cf1410 = customer.Working.IncomeMethod,
                                    Cf1412 = customer.Working.OtherLoans.Replace(",", string.Empty),
                                    Cf990 = customer.Loan.Term,
                                    Cf1210 = customer.Loan.Category,
                                    Cf1040 = customer.Loan.Product,
                                    Cf1220 = customer.Loan.BuyInsurance == true ? "1" : "0",
                                    Cf968 = amount.ToString(),
                                    Cf1424 = totalAmount.ToString(),
                                    Cf1054 = customer.Loan.SignAddress,

                                    Cf1414 = customer.SaleInfo.Name,
                                    Cf1422 = customer.SaleInfo.Code,
                                    Cf1418 = customer.SaleInfo.Phone,
                                    Cf1196 = customer.SaleInfo.Note + " ",

                                    Cf1208 = customer.ContractCode,
                                    Cf1420 = customer.Result != null ? customer.Result.Reason : "",
                                    Cf1052 = "-",
                                    Cf1036 = listTypeDocuments,
                                    Cf1426 = listLinkDocuments,
                                    SalesStage = "1.KH mới",
                                    Cf1184 = customer.Result != null ? customer.Result.ReturnStatus : "",
                                    Cf1188 = "-",
                                    AssignedUserId = "19x2629",
                                    Cf1256 = "-",
                                    Cf1264 = "????",
                                    Cf1230 = "",
                                    Id = customer.CRMId
                                };
                                var crmCustomer = PushDataToCRM(dataCRM, session, dataCRMProcessing);
                                if (crmCustomer?.Result?.Record != null && string.IsNullOrEmpty(customer.CRMId))
                                {
                                    customer.CRMId = crmCustomer.Result.Record.Id;
                                    _customerServices.ReplaceOne(customer);

                                }
                            }
                        }

                        // Update result to data crm processing
                        if(crmCustomerResponse?.Success == true)
                        {
                            dataCRMProcessing.FinishDate = DateTime.Now;
                            _dataCRMProcessingServices.UpdateByCustomerId(dataCRMProcessing, DataCRMProcessingStatus.Done);
                        }else
                        {
                            dataCRMProcessing.FinishDate = DateTime.Now;
                            dataCRMProcessing.Message = crmCustomerResponse?.Error?.Message ?? "";
                            _dataCRMProcessingServices.UpdateByCustomerId(dataCRMProcessing, DataCRMProcessingStatus.Error);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
            }
        }

        private UpSertCrmResponse PushDataToCRM(CRMBaseModel dataCRM, string session, DataCRMProcessing dataCRMProcessing)
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
                _logger.LogInformation("User was pushed to CRM: {0} - Status: {1}", dataCRMProcessing.CustomerId, Common.DataCRMProcessingStatus.Done);

                return JsonConvert.DeserializeObject<UpSertCrmResponse>(response.Content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

    }
}
