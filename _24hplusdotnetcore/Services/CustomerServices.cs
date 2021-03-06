﻿using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.CRM;
using _24hplusdotnetcore.Models.MC;
using _24hplusdotnetcore.Services.MC;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class CustomerServices
    {
        private readonly ILogger<CustomerServices> _logger;
        private readonly IMongoCollection<Customer> _customer;
        private readonly NotificationServices _notificationServices;
        private readonly UserRoleServices _userroleServices;
        private readonly CRM.DataCRMProcessingServices _dataCRMProcessingServices;
        private readonly MC.DataMCProcessingServices _dataMCProcessingServices;
        private readonly MCCheckCICService _mcCheckCICService;
        public CustomerServices(IMongoDbConnection connection,
        ILogger<CustomerServices> logger,
        NotificationServices notificationServices,
        UserRoleServices userroleServices,
        CRM.DataCRMProcessingServices dataCRMProcessingServices,
        MC.DataMCProcessingServices dataMCProcessingServices,
        MCCheckCICService mcCheckCICService)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _customer = database.GetCollection<Customer>(MongoCollection.CustomerCollection);
            _logger = logger;
            _notificationServices = notificationServices;
            _userroleServices = userroleServices;
            _dataCRMProcessingServices = dataCRMProcessingServices;
            _dataMCProcessingServices = dataMCProcessingServices;
            _mcCheckCICService = mcCheckCICService;
        }
        public List<Customer> GetList(string UserName, DateTime? DateFrom, DateTime? DateTo, string Status, string greentype, string customername, int? pagenumber, int? pagesize, ref int totalPage, ref int totalrecord)
        {
            var lstCustomer = new List<Customer>();
            DateTime _datefrom = DateFrom.HasValue ? Convert.ToDateTime(DateFrom) : new DateTime(0001, 01, 01);
            DateTime _dateto = DateTo.HasValue ? Convert.ToDateTime(DateTo) : new DateTime(9999, 01, 01);
            try
            {
                int _pagesize = !pagesize.HasValue ? Common.Config.PageSize : (int)pagesize;
                var filterUserName = Builders<Customer>.Filter.Regex(c => c.UserName, "/^" + UserName + "$/i");

                var filterCreateDate = Builders<Customer>.Filter.Gte(c => c.CreatedDate, _datefrom) & Builders<Customer>.Filter.Lte(c => c.CreatedDate, _dateto);
                filterUserName = filterUserName & filterCreateDate;
                if (!string.IsNullOrEmpty(greentype))
                {
                    var filterGreenType = Builders<Customer>.Filter.Eq(c => c.GreenType, greentype);
                    filterUserName = filterUserName & filterGreenType;
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    if (Status.ToUpper() == CustomerStatus.REJECT)
                    {
                        var filterStatus = Builders<Customer>.Filter.Or(
                            Builders<Customer>.Filter.Regex(c => c.Status, "/^Reject$/i"),
                            Builders<Customer>.Filter.Regex(c => c.Status, "/^Return$/i"));
                        filterUserName = filterUserName & filterStatus;
                    }
                    else
                    {
                        var filterStatus = Builders<Customer>.Filter.Regex(c => c.Status, "/^" + Status + "$/i");
                        filterUserName = filterUserName & filterStatus;
                    }
                }
                if (!string.IsNullOrEmpty(customername))
                {
                    var filterCustomerName = Builders<Customer>.Filter.Regex(c => c.Personal.Name, ".*" + customername + ".*");
                    filterUserName = filterUserName & filterCustomerName;
                }
                var lstCount = _customer.Find(filterUserName).SortBy(c => c.CreatedDate).ToList().Count;
                lstCustomer = _customer.Find(filterUserName).SortByDescending(c => c.ModifiedDate)
               .Skip((pagenumber != null && pagenumber > 0) ? ((pagenumber - 1) * _pagesize) : 0).Limit(_pagesize).ToList();
                totalrecord = lstCount;
                if (lstCount == 0)
                {
                    totalPage = 0;
                }
                else
                {
                    if (lstCount <= _pagesize)
                    {
                        totalPage = 1;
                    }
                    else
                    {
                        totalPage = lstCount / _pagesize + ((lstCount % _pagesize) > 0 ? 1 : 0);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstCustomer;
        }
        public Customer GetCustomer(string Id)
        {
            var objCustomer = new Customer();
            try
            {
                objCustomer = _customer.Find(c => c.Id == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objCustomer;
        }
        public List<Customer> GetCustomerByUserName(string UserName, int? pagenumber)
        {
            var lstCustomer = new List<Customer>();
            try
            {
                lstCustomer = _customer.Find(c => c.UserName == UserName).SortByDescending(c => c.ModifiedDate).Skip((pagenumber != null && pagenumber > 0) ? ((pagenumber - 1) * Common.Config.PageSize) : 0).Limit(Common.Config.PageSize).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstCustomer;
        }
        public Customer CreateCustomer(Customer customer)
        {
            try
            {
                customer.CreatedDate = Convert.ToDateTime(DateTime.Now);
                customer.ModifiedDate = Convert.ToDateTime(DateTime.Now);
                _customer.InsertOne(customer);
                if (customer.Status == CustomerStatus.SUBMIT)
                {
                    var dataCRMProcessing = new DataCRMProcessing
                    {
                        CustomerId = customer.Id,
                        Status = DataCRMProcessingStatus.InProgress,
                        LeadSource = LeadSourceType.MC.ToString()
                    };
                    _dataCRMProcessingServices.CreateOne(dataCRMProcessing);
                    if (customer.GreenType == GeenType.GreenC)
                    {
                        var dataMCProcessing = new DataMCProcessing
                        {
                            CustomerId = customer.Id,
                            Status = DataCRMProcessingStatus.InProgress
                        };
                        _dataMCProcessingServices.CreateOne(dataMCProcessing);
                    }
                }
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public long UpdateCustomerMCId(string customerId, int MCId)
        {
            long updateCount = 0;
            try
            {

                var customer = _customer.Find(c => c.Id == customerId).FirstOrDefault();
                customer.ModifiedDate = Convert.ToDateTime(DateTime.Now);
                customer.MCId = MCId;
                updateCount = _customer.ReplaceOne(c => c.Id == customer.Id, customer).ModifiedCount;

            }
            catch (Exception ex)
            {
                updateCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }
        public long UpdateCustomer(Customer customer)
        {
            long updateCount = 0;
            try
            {
                string teamLead = "";
                string message = "";
                string type = "";

                dynamic prvCustomer = _customer.Find(c => c.Id == customer.Id).FirstOrDefault();
                var currUser = _userroleServices.GetUserRoleByUserName(customer.UserName);
                if (currUser != null)
                {
                    teamLead = currUser.TeamLead;
                }
                customer.ModifiedDate = Convert.ToDateTime(DateTime.Now);
                customer.CreatedDate = prvCustomer.CreatedDate;
                if (customer.Documents == null)
                {
                    customer.Documents = prvCustomer.Documents;
                }
                if (customer.ReturnDocuments == null)
                {
                    customer.ReturnDocuments = prvCustomer.ReturnDocuments;
                }
                if (customer.Result == null)
                {
                    customer.Result = prvCustomer.Result;
                }
                customer.MCId = prvCustomer.MCId;
                customer.CRMId = prvCustomer.CRMId;
                customer.MCAppId = prvCustomer.MCAppId;
                customer.MCAppnumber = prvCustomer.MCAppnumber;
                customer.ContractCode = prvCustomer.ContractCode;
                updateCount = _customer.ReplaceOne(c => c.Id == customer.Id, customer).ModifiedCount;

                if (customer.Status.ToUpper() == CustomerStatus.SUBMIT)
                {
                    // Update to CRM
                    var dataCRMProcessing = new DataCRMProcessing
                    {
                        CustomerId = customer.Id,
                        Status = DataCRMProcessingStatus.InProgress,
                        LeadSource = LeadSourceType.MC.ToString()
                    };
                    _dataCRMProcessingServices.CreateOne(dataCRMProcessing);

                    // Notification
                    var cic = _mcCheckCICService.FindOneByIdentity(customer.Personal.IdCard);
                    if (cic != null && cic.Status == "SUCCESS")
                    {
                        if (MCCicMapping.APPROVE_CIC_RESULT_LIST.Where(x => x == cic.CicResult).Any())
                        {
                            type = NotificationType.Add;
                            message = string.Format(Message.NotificationAdd, customer.SaleInfo.Name, customer.Personal.Name);

                            var objNoti = new Notification
                            {
                                green = GeenType.GreenC,
                                recordId = customer.Id,
                                isRead = false,
                                type = type,
                                userName = teamLead,
                                message = message,
                                createAt = Convert.ToDateTime(DateTime.Today.ToLongDateString())
                            };
                            _notificationServices.CreateOne(objNoti);
                        }
                    }
                }
                else if (customer.Status.ToUpper() == CustomerStatus.APPROVE)
                {
                    // send data to MC
                    if (customer.GreenType == GeenType.GreenC)
                    {
                        var dataMCProcessing = new DataMCProcessing
                        {
                            CustomerId = customer.Id,
                            Status = DataCRMProcessingStatus.InProgress
                        };
                        _dataMCProcessingServices.CreateOne(dataMCProcessing);
                    }
                }
            }
            catch (Exception ex)
            {
                updateCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }
        public long DeleteCustomer(string[] Ids)
        {
            long DeleteCount = 0;
            try
            {
                for (int i = 0; i < Ids.Length; i++)
                {
                    DeleteCount += _customer.DeleteOne(c => c.Id == Ids[i] && c.Status.ToUpper() == Common.CustomerStatus.DRAFT).DeletedCount;
                }
            }
            catch (Exception ex)
            {
                DeleteCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return DeleteCount;
        }
        public StatusCount GetStatusCount(string userName, string GreenType)
        {
            var statusCount = new StatusCount();
            try
            {
                var lstCustomer = _customer.Find(c => c.UserName == userName && c.GreenType == GreenType).ToList();
                if (lstCustomer != null && lstCustomer.Count > 0)
                {
                    var statusdraft = lstCustomer.Where(l => string.Equals(l.Status, CustomerStatus.DRAFT, StringComparison.CurrentCultureIgnoreCase)).ToList().Count;

                    var statusreturn = lstCustomer.Where(l => string.Equals(l.Status, CustomerStatus.RETURN, StringComparison.CurrentCultureIgnoreCase)).ToList().Count;

                    var statussubmit = lstCustomer.Where(l => string.Equals(l.Status, CustomerStatus.SUBMIT, StringComparison.CurrentCultureIgnoreCase)).ToList().Count;

                    var statusreject = lstCustomer.Where(l => string.Equals(l.Status, CustomerStatus.REJECT, StringComparison.CurrentCultureIgnoreCase)).ToList().Count;

                    var statusapprove = lstCustomer.Where(l => string.Equals(l.Status, CustomerStatus.APPROVE, StringComparison.CurrentCultureIgnoreCase)).ToList().Count;

                    var all = lstCustomer.Count;

                    statusCount.Draft = statusdraft;
                    statusCount.Return = statusreturn + statusreject;
                    statusCount.Submit = statussubmit;
                    statusCount.Approve = statusapprove;
                    statusCount.All = all;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return statusCount;
        }
        internal long[] CustomerPagesize(List<Customer> lstCustomer)
        {
            long customersize = lstCustomer.Count;
            if (customersize <= Common.Config.PageSize)
            {
                return new long[]{
                    customersize,
                    1
                };
            }
            long totalpage = (customersize % Common.Config.PageSize) > 0 ? (customersize / Common.Config.PageSize + 1) : (customersize / Common.Config.PageSize + 1);
            return new long[]{
                customersize,
                totalpage
            };
        }

        public Customer GetCustomerByIdCard(string IdCard)
        {
            var customer = new Customer();
            try
            {
                customer = _customer.Find(c => c.Personal.IdCard == IdCard).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return customer;
        }

        public IEnumerable<Customer> GetListSubmitedCustomerByIdCard(string IdCard)
        {
            try
            {
                return _customer.Find(c => c.Personal.IdCard == IdCard && c.Status == CustomerStatus.SUBMIT).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IEnumerable<Customer> GetByIdCards(IEnumerable<string> idCards)
        {
            try
            {
                return _customer.Find(c => idCards.Contains(c.Personal.IdCard)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IEnumerable<Customer> GetByIds(IEnumerable<string> ids)
        {
            try
            {
                return _customer.Find(c => ids.Contains(c.Id)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public Customer GetByMCId(int mcId)
        {
            try
            {
                return _customer.Find(c => c.MCId == mcId).First();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<CustomerCheckListRequestModel> GetCustomerCheckListAsync(string id)
        {
            try
            {
                var filter = Builders<Customer>.Filter.Eq(c => c.Id, id);
                var unwindOption = new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true };
                var projectMapping = new BsonDocument()
                {
                   {"_id", 0 },
                   {"MobileSchemaProductCode", "$Loan.ProductObj.ProductCodeMC" },
                   {"MobileTemResidence", new BsonDocument("$toInt", "$IsTheSameResidentAddress") },
                   {"LoanAmountAfterInsurrance", "$Loan.Amount" },
                   {"ShopCode", "$Loan.SignAddress" },
                   {"CustomerName", "$Personal.Name" },
                   {"CitizenId", "$Personal.IdCard" },
                   {"LoanTenor", "$Loan.Term" },
                   {"HasInsurance", "$Loan.BuyInsurance" },
                   {"CompanyTaxNumber", "$Working.TaxId" },
                };
                BsonDocument document = await _customer
                    .Aggregate()
                    .Match(filter)
                    .Lookup("Product", "Loan.ProductId", "ProductId", "Loan.ProductObj")
                    .Unwind("Loan.ProductObj", unwindOption)
                    .Project(projectMapping)
                    .FirstOrDefaultAsync();

                var result = BsonSerializer.Deserialize<CustomerCheckListRequestModel>(document);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public List<Customer> InsertMany(List<Customer> customers)
        {
            _customer.InsertMany(customers);
            return customers;
        }

        public long UpdateStatus(CustomerUpdateStatusDto dto)
        {
            long updateCount = 0;
            try
            {
                var customer = _customer.Find(x => x.Id == dto.CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    string sender = dto.LeadSource + string.Empty;
                    string userName = "";
                    string message = "";
                    string type = "";

                    var currUser = _userroleServices.GetUserRoleByUserName(customer.UserName);
                    if (currUser != null && sender != "")
                    {
                        sender = currUser.TeamLead;
                    }
                    customer.Status = dto.Status;
                    if (customer.Result == null)
                    {
                        customer.Result = new Models.Result();
                        customer.Result.Reason = dto.Reason;
                        customer.Result.ReturnStatus = dto.ReturnStatus;
                    }
                    else
                    {
                        customer.Result.Reason = dto.Reason;
                        customer.Result.ReturnStatus = dto.ReturnStatus;
                    }
                    customer.ModifiedDate = Convert.ToDateTime(DateTime.Now);
                    updateCount = _customer.ReplaceOne(c => c.Id == customer.Id, customer).ModifiedCount;

                    if (customer.Status.ToUpper() == CustomerStatus.REJECT)
                    {
                        userName = customer.UserName;
                        type = NotificationType.Reject;
                        message = string.Format(Message.NotificationReject, sender, customer.Personal.Name);
                    }
                    else if (customer.Status.ToUpper() == CustomerStatus.APPROVE)
                    {
                        // send data to MC
                        if (customer.GreenType == GeenType.GreenC)
                        {
                            var dataMCProcessing = new DataMCProcessing
                            {
                                CustomerId = customer.Id,
                                Status = DataCRMProcessingStatus.InProgress
                            };
                            _dataMCProcessingServices.CreateOne(dataMCProcessing);
                        }
                        userName = customer.UserName;
                        type = NotificationType.Approve;
                        message = string.Format(Message.TeamLeadApprove, sender, customer.Personal.Name);
                    }
                    else if (customer.Status.ToUpper() == CustomerStatus.RETURN)
                    {
                        userName = customer.UserName;
                        type = NotificationType.Reject;
                        message = string.Format(Message.NotificationReturn, sender, customer.Personal.Name);
                    }
                    else if (customer.Status.ToUpper() == CustomerStatus.CANCEL)
                    {
                        userName = customer.UserName;
                        type = NotificationType.Reject;
                        message = string.Format(Message.NotificationCancel, sender, customer.Personal.Name);
                    }
                    else if (customer.Status.ToUpper() == CustomerStatus.SUCCESS)
                    {
                        userName = customer.UserName;
                        type = NotificationType.Approve;
                        message = string.Format(Message.NotificationSuccess, sender, customer.Personal.Name);
                    }

                    // Update to CRM
                    var dataCRMProcessing = new DataCRMProcessing
                    {
                        CustomerId = customer.Id,
                        Status = DataCRMProcessingStatus.InProgress,
                        LeadSource = LeadSourceType.MC.ToString()
                    };
                    _dataCRMProcessingServices.CreateOne(dataCRMProcessing);

                    if (message != "")
                    {
                        var objNoti = new Notification
                        {
                            green = GeenType.GreenC,
                            recordId = customer.Id,
                            isRead = false,
                            type = type,
                            userName = userName,
                            message = message,
                            createAt = Convert.ToDateTime(DateTime.Today.ToLongDateString())
                        };
                        _notificationServices.CreateOne(objNoti);
                    }
                }
            }
            catch (Exception ex)
            {
                updateCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }

        public long UpdateMCAppId(MCNotificationDto noti)
        {
            long updateCount = 0;
            try
            {
                var customer = _customer.Find(x => x.MCId == noti.Id).FirstOrDefault();
                if (customer != null)
                {
                    customer.MCAppId = noti.AppId;
                    customer.MCAppnumber = Int32.Parse(noti.AppNumber);
                    customer.ContractCode = "MC-" + customer.MCAppnumber;
                    customer.ModifiedDate = Convert.ToDateTime(DateTime.Now);
                    updateCount = _customer.ReplaceOne(c => c.Id == customer.Id, customer).ModifiedCount;

                }
            }
            catch (Exception ex)
            {
                updateCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }

        public long UpdateCustomerMCReturnDocuments(string customerId, IEnumerable<GroupDocument> documents)
        {
            long updateCount = 0;
            try
            {

                var customer = _customer.Find(c => c.Id == customerId).FirstOrDefault();
                customer.ModifiedDate = Convert.ToDateTime(DateTime.Now);
                customer.ReturnDocuments = documents;
                updateCount = _customer.ReplaceOne(c => c.Id == customer.Id, customer).ModifiedCount;

            }
            catch (Exception ex)
            {
                updateCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }

        public void ReplaceOne(Customer customer)
        {
            try
            {
                _customer.ReplaceOne(c => c.Id == customer.Id, customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
