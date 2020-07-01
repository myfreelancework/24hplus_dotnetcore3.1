
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
        private readonly IRestMCService _restMCService;
        public MCService(ILogger<MCService> logger, FileUploadServices fileUploadServices, IWebHostEnvironment webHostEnvironment, DataMCProcessingServices dataMCProcessingServices, CustomerServices customerServices, IRestMCService restMCService)
        {
            _logger = logger;
            _fileUploadServices = fileUploadServices;
            _hostingEnvironment = webHostEnvironment;
            _dataMCProcessingServices = dataMCProcessingServices;
            _restMCService = restMCService;
            _customerServices = customerServices;
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
                        var lstFileUpload = _fileUploadServices.GetListFileUploadByCustomerId(item.CustomerId);
                        lstFileUpload = lstFileUpload.Where(l => !string.IsNullOrEmpty(l.DocumentCode)).ToList();
                        var fileZipInfo = ZipFiles(lstFileUpload, objCustomer.Id);
                        var filePath = fileZipInfo[0];
                        var hash = fileZipInfo[1];
                        var dataMC = new DataMC();
                        dataMC.AppStatus = "1";
                        dataMC.Request = new Models.MC.Request();
                        dataMC.Request.CitizenId = objCustomer.Personal.IdCard;
                        dataMC.Request.CustomerName = objCustomer.Personal.Name;
                        dataMC.Request.ProductId = objCustomer.Loan.ProductId;
                        dataMC.Request.SaleCode = objCustomer.UserName;
                        dataMC.Request.CompanyTaxNumber = objCustomer.Working.TaxId;
                        dataMC.Request.ShopCode = objCustomer.SaleInfo.Code;
                        dataMC.Request.LoanAmount = objCustomer.Loan.Amount;
                        dataMC.Request.LoanTenor = objCustomer.Loan.Term;
                        dataMC.Request.HasInsurance = objCustomer.Loan.BuyInsurance;
                        dataMC.Md5 = hash;
                        dataMC.Info = new List<Info>();
                        
                        foreach (var f in lstFileUpload)
                        {
                            var dataMCFileInfo = new Info();
                            dataMCFileInfo.DocumentCode = f.DocumentCode;
                            dataMCFileInfo.FileName = f.FileUploadName;
                            dataMCFileInfo.MimeType = "jpg";
                            dataMCFileInfo.GroupId = f.GroupId;
                            dataMC.Info.Add(dataMCFileInfo);
                        }
                        var client = new RestClient(Url.MC_BASE_URL + Url.MC_UPLOAD_DOCUMENT);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Authorization", "Bearer "+token+"");
                        request.AddHeader("x-security", "MEKONG-CREDIT-57d733a9-bcb5-4bff-aca1-f58163122fae");
                        request.AddHeader("Content-Type", "multipart/form-data");
                        request.AddFile("file", ""+ filePath +"");
                        request.AddParameter("object", JsonConvert.SerializeObject(dataMC));
                        IRestResponse response = client.Execute(request);
                        _logger.LogInformation(response.Content);
                        if (!string.IsNullOrEmpty(response.Content))
                        {
                            uploadCount ++;
                        }
                        File.Delete(filePath);
                        _dataMCProcessingServices.UpdateByCustomerId(item.CustomerId, Common.DataCRMProcessingStatus.Done);
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
        
        public string[] ZipFiles(List<FileUpload> listFile, string customerId)
        {
            try
            {
                //var listFile = _fileUploadServices.GetListFileUploadByCustomerId(customerId);
                string serverPath = Path.Combine(_hostingEnvironment.ContentRootPath, "FileUpload");
                if (listFile.Count > 0)
                {
                    Directory.CreateDirectory(Path.Combine(serverPath, customerId));
                    string d = Path.Combine(serverPath, customerId);
                    for (int i = 0; i < listFile.Count; i++)
                    {
                        string s = Path.Combine(serverPath, listFile[i].FileUploadName);
                        File.Copy(s, Path.Combine(d, listFile[i].FileUploadName), true);
                    }
                    ZipFile.CreateFromDirectory(d, Path.Combine(serverPath, customerId + ".zip"));
                    string fileZip = Path.Combine(serverPath, customerId + ".zip");
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
                    request.AddHeader("Authorization", "Bearer "+token+"");
                    request.AddHeader("x-security", "MEKONG-CREDIT-57d733a9-bcb5-4bff-aca1-f58163122fae");
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
    }
}
