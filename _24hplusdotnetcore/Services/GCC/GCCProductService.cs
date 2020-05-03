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
    public class GCCProductService
    {
        private readonly ILogger<GCCProductService> _logger;
        private readonly IMongoCollection<GCCPersonalProduct> _collection;
        public GCCProductService(IMongoDbConnection connection,ILogger<GCCProductService> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _collection = database.GetCollection<GCCPersonalProduct>(MongoCollection.GCCProduct);
            _logger = logger;
        }
        public GCCPersonalProduct FindOneByProductName(string productName)
        {
            try
            {
                return _collection.Find(x => x.product_name == productName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public string MappingPackage(string key)
        {
            var result = "";
            switch (key)
            {
                case "Gói cơ bản":
                    result = "co_ban";
                    break;
                case "Gói thiết yếu":
                    result = "thiet_yeu";
                    break;
            }
            return result;
        }
    }
}
