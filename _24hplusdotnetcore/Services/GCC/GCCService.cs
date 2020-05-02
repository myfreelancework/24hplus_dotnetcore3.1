using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.GCC;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace _24hplusdotnetcore.Services.GCC
{
    public class GCCService
    {
        private readonly ILogger<GCCService> _logger;
        private readonly IMongoCollection<GCCPersonalInsurance> _collection;
        public GCCService(IMongoDbConnection connection, ILogger<GCCService> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _collection = database.GetCollection<GCCPersonalInsurance>(MongoCollection.GCCPersonalInsurance);
            _logger = logger;
        }
        public string GetToken()
        {
            try
            {
                string key = "";
                var url = Url.GCC_BASE_URL + string.Format(Url.GCC_GET_SSO_KEY, ConfigRequest.GCC_CLIENT_CODE, ConfigRequest.GCC_CLIENT_SECRET);
                dynamic request = HttpRequest(url, Method.GET, null, null);
                if (request != null && request.result != null && request.result.key != null)
                {
                    key = request.result.key;
                }
                return key;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return "";
            }
        }

        public dynamic SendInfo(string key, dynamic body)
        {
            try
            {
                var url = Url.GCC_BASE_URL + Url.GCC_PUSH_DATA;
                dynamic request = HttpRequest(url, Method.POST, key, body);
                if (request != null && request.code != null)
                {
                    return request;
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


        public GCCPersonalInsurance FindOneByRequestCode(string requestCode)
        {
            try
            {
                return _collection.Find(x => x.request_code == requestCode).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public GCCPersonalInsurance FindOne(string id)
        {
            try
            {
                return _collection.Find(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public GCCPersonalInsurance CreateOne(GCCPersonalInsurance obj)
        {
            try
            {
                obj.CreateDate = Convert.ToDateTime(DateTime.Now.ToLocalTime());
                _collection.InsertOne(obj);
                return obj;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public long UpdateOne(GCCPersonalInsurance noti)
        {
            long updateCount = 0;
            try
            {
                updateCount = _collection.ReplaceOne(c => c.Id == noti.Id, noti).ModifiedCount;
            }
            catch (Exception ex)
            {
                updateCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }



        private dynamic HttpRequest(string Url, dynamic Method, string key, dynamic body)
        {
            try
            {
                var client = new RestClient(Url);
                var request = new RestRequest(Method);

                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("client_code", ConfigRequest.GCC_CLIENT_CODE);
                request.AddHeader("client_secret", ConfigRequest.GCC_CLIENT_SECRET);
                request.AddHeader("key", key);
                if (body != null)
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
    }
}
