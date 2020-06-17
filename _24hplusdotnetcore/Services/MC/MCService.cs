
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.Models.MC;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace _24hplusdotnetcore.Services.MC
{
    public class MCService
    {
        private readonly ILogger<MCService> _logger;
        public MCService(ILogger<MCService> logger)
        {
            _logger = logger;
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
    }
}
