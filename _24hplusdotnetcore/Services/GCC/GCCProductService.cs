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
        public GCCPersonalProduct FindOneByProductCode(string productCode)
        {
            try
            {
                var a = _collection.Find(x => x.Id == "5eaa64173a389b00074bca79").FirstOrDefault();
                return _collection.Find(x => x.product_code == productCode).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
