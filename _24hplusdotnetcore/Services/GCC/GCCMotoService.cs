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
    public class GCCMotoService
    {
        private readonly ILogger<GCCMotoService> _logger;
        private readonly IMongoCollection<GCCMotoInsuranceModel> _collection;
        public GCCMotoService(IMongoDbConnection connection, ILogger<GCCMotoService> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _collection = database.GetCollection<GCCMotoInsuranceModel>(MongoCollection.GCCMotoInsurance);
            _logger = logger;
        }

        public dynamic SendInfo(string key, GCCMotoInsuranceModel body)
        {
            try
            {
                var url = Url.GCC_BASE_URL + string.Format(Url.GCC_PUSH_DATA, ConfigRequest.GCC_CLIENT_SECRET, key);
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


        public GCCMotoInsuranceModel FindOneByRequestCode(string requestCode)
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
        public GCCMotoInsuranceModel FindOne(string id)
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
        public GCCMotoInsuranceModel CreateOne(GCCMotoInsuranceModel obj)
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
        public long UpdateOne(GCCMotoInsuranceModel noti)
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


        private dynamic HttpRequest(string Url, dynamic Method, string key, GCCMotoInsuranceModel body)
        {
            try
            {
                var client = new RestClient(Url);
                var request = new RestRequest(Method);

                if (body != null)
                {
                        string[] images = {};
                        dynamic motoInfo = new
                        {
                            info = new
                            {
                                bsx = body.data.info.bsx,
                                sokhung = body.data.info.sokhung,
                                somay = body.data.info.somay
                            },
                            images = images
                        };

                        string data = JsonConvert.SerializeObject(motoInfo);
                        string package = JsonConvert.SerializeObject(body.package);

                        request.AlwaysMultipartFormData = true;
                        request.AddParameter("product_code", body.product_code);
                        request.AddParameter("agency_id", body.agency_id);
                        request.AddParameter("program", body.program);
                        request.AddParameter("package", body.package);
                        request.AddParameter("request_code", body.request_code);
                        request.AddParameter("buy_fullname", body.buy_fullname);
                        request.AddParameter("buy_bod", body.buy_bod);
                        request.AddParameter("buy_address", body.buy_address);
                        request.AddParameter("buy_phone", body.buy_phone);
                        request.AddParameter("buy_cmnd", body.buy_cmnd);
                        request.AddParameter("buy_email", body.buy_email);
                        request.AddParameter("buy_gender", body.buy_gender);
                        request.AddParameter("url_callback", body.url_callback);
                        request.AddParameter("data", data);
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
