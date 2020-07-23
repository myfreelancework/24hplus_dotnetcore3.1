
using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.Common.Enums;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MC
{
    public class MCService
    {
        private readonly ILogger<MCService> _logger;
        private readonly FileUploadServices _fileUploadServices;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly DataMCProcessingServices _dataMCProcessingServices;
        private readonly CustomerServices _customerServices;
        private readonly ProductServices _productServices;
        private readonly IRestMCService _restMCService;
        private readonly IMapper _mapper;

        public MCService(ILogger<MCService> logger,
        FileUploadServices fileUploadServices,
        IWebHostEnvironment webHostEnvironment,
        DataMCProcessingServices dataMCProcessingServices,
        CustomerServices customerServices,
        ProductServices productServices,
        IRestMCService restMCService,
        IMapper mapper)
        {
            _logger = logger;
            _fileUploadServices = fileUploadServices;
            _hostingEnvironment = webHostEnvironment;
            _dataMCProcessingServices = dataMCProcessingServices;
            _restMCService = restMCService;
            _customerServices = customerServices;
            _productServices = productServices;
            _mapper = mapper;
        }
        public dynamic CheckDuplicate(string citizenID)
        {
            try
            {
                var token = GetMCToken();
                if (string.IsNullOrEmpty(token))
                {
                    return null;
                }
                else
                {
                    var client = new RestClient(Url.MC_BASE_URL + string.Format(Url.MC_CHECK_DUPLICATE, citizenID));
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + token + "");
                    request.AddHeader("x-security", "" + ConfigRequest.MC_Security_Key + "");
                    IRestResponse response = client.Execute(request);
                    dynamic content = JsonConvert.DeserializeObject(response.Content);
                    return content;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public dynamic CheckInfo(string citizenID, string customerName)
        {
            try
            {
                string token = GetMCToken();
                if (!string.IsNullOrEmpty(token))
                {
                    var client = new RestClient(Url.MC_BASE_URL + string.Format(Url.MC_CHECK_INFO, citizenID, customerName));
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + token + "");
                    request.AddHeader("x-security", "" + Common.Config.CredMC_Security_Key + "");
                    IRestResponse response = client.Execute(request);
                    List<dynamic> content = JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
                    return content.ToArray()[0];
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
        public dynamic CheckCAT(string companyTaxNumber)
        {
            try
            {
                string token = GetMCToken();
                if (!string.IsNullOrEmpty(token))
                {
                    var client = new RestClient(Url.MC_BASE_URL + string.Format(Url.MC_CHECK_CAT, companyTaxNumber));
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + token + "");
                    request.AddHeader("x-security", "" + Common.Config.CredMC_Security_Key + "");
                    IRestResponse response = client.Execute(request);
                    dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    return content;
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
        public string GetMCToken()
        {
            string token = "";
            try
            {
                var client = new RestClient("" + Url.MC_BASE_URL + Url.MC_LOGIN + "");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("x-security", "" + Common.Config.CredMC_Security_Key + "");
                request.AddParameter("application/json", "{\n    \"username\": \"" + Common.Config.CredMC_Username + "\",\n    \"password\": \"" + Common.Config.CredMC_Password + "\",\n    \"notificationId\": \"notificationId.mekongcredit.3rd\",\n    \"imei\": \"imei.mekongcredit.3rd\",\n    \"osType\": \"ANDROID\"\n}", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);
                if (content.token != null)
                {
                    token = content.token;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return token;
        }
        public int PushDataToMC()
        {
            try
            {
                var lstMCProcessing = _dataMCProcessingServices.GetDataMCProcessings(Common.DataCRMProcessingStatus.InProgress);
                var token = GetMCToken();//"ca5d2357-90be-4bd9-b9d9-732c690dd9c3";//GetMCToken();
                int uploadCount = 0;
                if (lstMCProcessing.Count > 0 && !string.IsNullOrEmpty(token))
                {
                    foreach (var item in lstMCProcessing)
                    {
                        var objCustomer = _customerServices.GetCustomer(item.CustomerId);
                        var product = _productServices.GetProductByProductId(objCustomer.Loan.ProductId);
                        var listInfo = new List<Info>();
                        List<string> listFile = new List<string>();
                        var documents = objCustomer.MCId != 0 ? objCustomer.ReturnDocuments : objCustomer.Documents;
                        
                        foreach (var group in documents)
                        {
                            var groupId = group.GroupId;
                            foreach (var doc in group.Documents)
                            {
                                foreach (var media in doc.UploadedMedias)
                                {
                                    string mimeType = "";
                                    if (media.Type != null && media.Type.IndexOf("/") > -1)
                                    {
                                        mimeType = media.Type.Split("/")[1];
                                    }
                                    else
                                    {
                                        mimeType = media.Type;
                                    }
                                    var dataMCFileInfo = new Info();
                                    dataMCFileInfo.GroupId = group.GroupId.ToString();
                                    dataMCFileInfo.DocumentCode = doc.DocumentCode;
                                    dataMCFileInfo.FileName = media.Name;
                                    dataMCFileInfo.MimeType = mimeType;
                                    listInfo.Add(dataMCFileInfo);
                                    listFile.Add(media.Name);
                                }
                            }
                        }
                        
                        var fileZipInfo = ZipFiles(objCustomer.Id, listFile);
                        var filePath = fileZipInfo[0];
                        var hash = fileZipInfo[1];
                        var loanAmount = objCustomer.Loan.Amount.Replace(",", string.Empty);
                        var dataMC = new DataMC();
                        dataMC.Request = new Models.MC.Request();
                        dataMC.Request.Id = objCustomer.MCId != 0 ? objCustomer.MCId : 0;
                        dataMC.Request.CitizenId = objCustomer.Personal.IdCard;
                        dataMC.Request.CustomerName = objCustomer.Personal.Name;
                        dataMC.Request.ProductId = product.ProductIdMC;
                        dataMC.Request.TempResidence = objCustomer.IsTheSameResidentAddress == true ? 1 : 0;
                        dataMC.Request.SaleCode = Config.MC_TLS_SALECODE;
                        dataMC.Request.CompanyTaxNumber = objCustomer.Working.TaxId;
                        dataMC.Request.ShopCode = objCustomer.Loan.SignAddress.Split('-')[0];
                        dataMC.Request.IssuePlace = objCustomer.Loan.SignAddress.Split('-')[1];
                        dataMC.Request.LoanAmount = Int32.Parse(loanAmount);
                        dataMC.Request.LoanTenor = Int16.Parse(objCustomer.Loan.Term);
                        dataMC.Request.HasInsurance = objCustomer.Loan.BuyInsurance.Equals("true") ? 1 : 0;
                        dataMC.Request.HasCourier = 0;

                        dataMC.AppStatus = objCustomer.MCId != 0 ? 2 : 1;
                        dataMC.MobileIssueDateCitizen = objCustomer.Personal.IdCardDate;
                        dataMC.MobileProductType = "CashLoan";
                        dataMC.Md5 = hash;
                        dataMC.Info = listInfo;

                        var client = new RestClient(Url.MC_BASE_URL + Url.MC_UPLOAD_DOCUMENT);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Authorization", "Bearer " + token + "");
                        request.AddHeader("x-security", Config.CredMC_Security_Key);
                        request.AddHeader("Content-Type", "multipart/form-data");
                        request.AddFile("file", "" + filePath + "");
                        request.AddParameter("object", JsonConvert.SerializeObject(dataMC));
                        IRestResponse response = client.Execute(request);
                        _logger.LogInformation(response.Content);
                        dynamic content = JsonConvert.DeserializeObject<dynamic>(response.Content);
                        if (content.id != null)
                        {
                            int mcId = Int32.Parse(content.id.Value.ToString());
                            _customerServices.UpdateCustomerMCId(objCustomer.Id, mcId);
                            uploadCount++;
                        }
                        else
                        {
                            CustomerUpdateStatusDto error = new CustomerUpdateStatusDto();
                            error.CustomerId = objCustomer.Id;
                            error.Status = CustomerStatus.REJECT;
                            error.LeadSource = LeadSourceType.MC.ToString();
                            error.Reason = content.returnMes != null ? content.returnMes.Value : "Lỗi gửi data qua MC";
                            _customerServices.UpdateStatus(error);
                        }
                        _dataMCProcessingServices.UpdateById(item.Id, Common.DataCRMProcessingStatus.Done);
                        File.Delete(filePath);
                    }
                }
                return uploadCount;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return -1;
            }
        }

        public string[] ZipFiles(string customerId, List<string> listFile)
        {
            try
            {
                //var listFile = _fileUploadServices.GetListFileUploadByCustomerId(customerId);
                string serverPath = Path.Combine(_hostingEnvironment.ContentRootPath, "FileUpload/" + customerId);
                String timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                Directory.CreateDirectory(Path.Combine(serverPath, timeStamp));
                string d = Path.Combine(serverPath, timeStamp);
                for (int i = 0; i < listFile.Count; i++)
                {
                   string s = Path.Combine(serverPath, listFile[i]);
                   File.Copy(s, Path.Combine(d, listFile[i]), true);
                }
                ZipFile.CreateFromDirectory(d, Path.Combine(serverPath, timeStamp + ".zip"), CompressionLevel.Optimal, false);
                string fileZip = Path.Combine(serverPath, timeStamp + ".zip");
                var md5 = MD5.Create();
                var stream = File.OpenRead(fileZip);
                var hash = md5.ComputeHash(stream);
                var hashString = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                return new string[]
                {
                        fileZip,
                        hashString
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        private dynamic SendRequest(string Url, string Method, dynamic body)
        {
            try
            {
                var client = new RestClient(Url);
                var request = new RestRequest(Method);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("x-security", "" + ConfigRequest.MC_Security_Key + "");
                if (body)
                {
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                }
                IRestResponse response = client.Execute(request);
                return JsonConvert.DeserializeObject<dynamic>(response.Content);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public List<MCProduct> GetProduct()
        {
            try
            {
                string token = GetMCToken();
                if (!string.IsNullOrEmpty(token))
                {
                    var client = new RestClient(Url.MC_BASE_URL + Url.MC_GET_PRODUCT);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + token + "");
                    request.AddHeader("x-security", Config.CredMC_Security_Key);
                    IRestResponse response = client.Execute(request);
                    Console.WriteLine(response.Content);
                    List<MCProduct> content = JsonConvert.DeserializeObject<List<MCProduct>>(response.Content);
                    return content;
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

        public async Task<CustomerCheckListResponseModel> CheckListAsync(string customerId)
        {
            try
            {
                CustomerCheckListRequestModel customerCheckList = await _customerServices.GetCustomerCheckListAsync(customerId);
                customerCheckList.HasCourier = 0;
                CustomerCheckListResponseModel result = await _restMCService.CheckListAsync(customerCheckList);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<KiosModel>> GetKiosAsync()
        {
            try
            {
                IEnumerable<KiosModel> kios = await _restMCService.GetKiosAsync();
                return kios;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<MCSuccessResponseDto> CancelCaseAsync(CancelCaseRequestDto cancelCaseRequestDto)
        {
            try
            {
                Customer customer = _customerServices.GetCustomer(cancelCaseRequestDto.CustomerId);
                if (customer == null)
                {
                    throw new ArgumentException(Message.CUSTOMER_NOT_FOUND);
                }

                MCCancelCaseRequestDto mCCancelCaseRequestDto = new MCCancelCaseRequestDto
                {
                    Id = customer.MCId,
                    Comment = cancelCaseRequestDto.Comment,
                    Reason = cancelCaseRequestDto.Reason
                };

                MCSuccessResponseDto mCSuccessResponseDto = await _restMCService.CancelCaseAsync(mCCancelCaseRequestDto);
                return mCSuccessResponseDto;
            }
            catch (Refit.ApiException ex)
            {
                _logger.LogError(ex, ex.Content);
                var error = await ex.GetContentAsAsync<MCErrorResponseDto>();
                throw new ArgumentException(error.ReturnMes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<MCCaseNoteListDto> GetCaseNoteAsync(string customerId)
        {
            try
            {
                Customer customer = _customerServices.GetCustomer(customerId);
                if (customer == null)
                {
                    throw new ArgumentException(Message.CUSTOMER_NOT_FOUND);
                }

                MCCaseNoteListDto mCCaseNoteListDto = await _restMCService.GetCaseNoteAsync(customer.MCAppnumber);
                return mCCaseNoteListDto;
            }
            catch (Refit.ApiException ex)
            {
                _logger.LogError(ex, ex.Content);
                var error = await ex.GetContentAsAsync<MCErrorResponseDto>();
                throw new ArgumentException(error.ReturnMes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<MCSuccessResponseDto> SendCaseNoteAsync(SendCaseNoteRequestDto sendCaseNoteRequestDto)
        {
            try
            {
                Customer customer = _customerServices.GetCustomer(sendCaseNoteRequestDto.CustomerId);
                if (customer == null)
                {
                    throw new ArgumentException(Message.CUSTOMER_NOT_FOUND);
                }

                var mCSendCaseNoteRequestDto = new MCSendCaseNoteRequestDto
                {
                    AppNumber = customer.MCAppnumber,
                    NoteContent = sendCaseNoteRequestDto.NoteContent
                };

                MCSuccessResponseDto mCSuccessResponseDto = await _restMCService.SendCaseNoteAsync(mCSendCaseNoteRequestDto);
                return mCSuccessResponseDto;
            }
            catch (Refit.ApiException ex)
            {
                _logger.LogError(ex, ex.Content);
                var error = await ex.GetContentAsAsync<MCErrorResponseDto>();
                throw new ArgumentException(error.ReturnMes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<CustomerCheckListResponseModel> GetReturnCheckListAsync(string customerId)
        {
            try
            {
                Customer customer = _customerServices.GetCustomer(customerId);
                if (customer == null)
                {
                    throw new ArgumentException(Message.CUSTOMER_NOT_FOUND);
                }

                CustomerCheckListResponseModel customerCheckListResponseModel = await _restMCService.GetReturnCheckListAsync(customer.MCAppId);
                return customerCheckListResponseModel;
            }
            catch (Refit.ApiException ex)
            {
                _logger.LogError(ex, ex.Content);
                var error = await ex.GetContentAsAsync<MCErrorResponseDto>();
                throw new ArgumentException(error.ReturnMes);
            }
        }
        public async Task<IEnumerable<GetCaseMCResponseDto>> GetCasesAsync(GetCaseRequestDto getCaseRequestDto)
        {
            try
            {
                GetCaseMCRequestDto request = _mapper.Map<GetCaseMCRequestDto>(getCaseRequestDto);

                IEnumerable<GetCaseMCResponseDto> cases = await _restMCService.GetCasesAsync(request);

                return cases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
