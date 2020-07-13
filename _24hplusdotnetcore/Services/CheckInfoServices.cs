using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models.MC;
using _24hplusdotnetcore.Services.MC;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class CheckInfoServices
    {
        private readonly ILogger<CheckInfoServices> _logger;
        private readonly MCService _mcServices;
        private readonly MCCheckCICService _mcCheckCICService;

        public CheckInfoServices(
            ILogger<CheckInfoServices> logger,
            MCCheckCICService mcCheckCICService,
            MCService mcServices)
        {
            _logger = logger;
            _mcServices = mcServices;
            _mcCheckCICService = mcCheckCICService;
        }
        public dynamic CheckInfoByType(string greentype, string citizenID, string customerName)
        {
            try
            {
                if (greentype.ToUpper() == "C")
                {
                    return CheckInforFromMC(citizenID, customerName);
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
        public dynamic CheckDuplicateByType(string greentype, string citizenID)
        {
            try
            {
                if (greentype.ToUpper() == "C")
                {
                    return CheckMCCitizendId(citizenID);
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

        private dynamic CheckMCCitizendId(string citizenID)
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
                    var client = new RestClient(string.Format(Common.Config.MC_CheckDuplicate_URL, citizenID));
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + token + "");
                    request.AddHeader("x-security", "" + Common.Config.CredMC_Security_Key + "");
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

        public dynamic CheckInforFromMC(string citizenID, string customerName)
        {
            try
            {
                string token = GetMCToken();
                if (!string.IsNullOrEmpty(token))
                {
                    var client = new RestClient(string.Format(Common.Config.MC_CheckInfo_URL, citizenID, customerName));
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Content-type", "application/json");
                    request.AddHeader("Authorization", "Bearer " + token + "");
                    request.AddHeader("x-security", "" + Common.Config.CredMC_Security_Key + "");
                    IRestResponse response = client.Execute(request);
                    List<dynamic> content = JsonConvert.DeserializeObject<List<dynamic>>(response.Content);
                    var result = content.ToArray()[0];
                    if (result.status == "NEW")
                    {
                        var cic = new MCCheckCICModel();
                        cic.RequestId = result.requestId;
                        cic.Identifier = result.identifier;
                        cic.CustomerName = result.customerName;
                        cic.CicResult = result.cicResult;
                        cic.Description = result.description;
                        cic.CicImageLink = result.cicImageLink;
                        cic.LastUpdateTime = result.lastUpdateTime;
                        cic.Status = result.status;
                        cic.CreateDate = Convert.ToDateTime(DateTime.Now);
                        _mcCheckCICService.CreateOne(cic);
                    }
                    else if (result.status == "CHECKING")
                    {
                        var oldCic = _mcCheckCICService.FindOneByIdentity(result.identifier);
                        if (oldCic == null)
                        {
                            var cic = new MCCheckCICModel();
                            cic.RequestId = result.requestId;
                            cic.Identifier = result.identifier;
                            cic.CustomerName = result.customerName;
                            cic.CicResult = result.cicResult;
                            cic.Description = result.description;
                            cic.CicImageLink = result.cicImageLink;
                            cic.LastUpdateTime = result.lastUpdateTime;
                            cic.Status = result.status;
                            cic.CreateDate = Convert.ToDateTime(DateTime.Now);
                            _mcCheckCICService.CreateOne(cic);
                        }
                    }

                    return result;
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
        public dynamic CheckCAT(string GreenType, string companyTaxNumber)
        {
            try
            {
                if (GreenType.ToUpper() == Common.GeenType.GreenC)
                {
                    return _mcServices.CheckCAT(companyTaxNumber);
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
                var client = new RestClient("" + Common.Config.CredMC_URL + "");
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
    }
}
