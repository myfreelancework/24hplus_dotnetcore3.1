using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using _24hplusdotnetcore.Models.CRM;
using System.Linq;
using _24hplusdotnetcore.Models;

namespace _24hplusdotnetcore.Services.CRM
{
    public class CRMServices
    {
        private readonly ILogger<CRMServices> _logger;
        public CRMServices(ILogger<CRMServices> logger)
        {
            _logger = logger;
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
                request.AddParameter("username", ""+Common.Constants.ConfigRequest.CRM_UserName+"");
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
                request.AddParameter("_session", ""+ session + "");
                request.AddParameter("query", ""+ queryString + "");
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
            try
            {
                
                var arrCustomer = crmCustomer.Result.Records;
                
                if (arrCustomer.Length > 0)
                {
                    foreach (var item in arrCustomer)
                    {
                        Customer objCustomer = new Customer();
                        objCustomer.Personal.Name = item.Potentialname;
                        objCustomer.Personal.Gender = item.Cf1026;
                        objCustomer.Personal.Phone = item.Cf854;
                        objCustomer.Personal.IdCard = item.Cf1050;
                        objCustomer.Personal.Email = item.Cf1028;

                    }
                }
                return 1;
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
    }
}
