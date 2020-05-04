using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _24hplusdotnetcore.Services
{
    public class ConfigServices
    {
        private readonly ILogger<NotificationServices> _logger;
        private readonly IMongoCollection<ConfigModel> _collection;
        public ConfigServices(IMongoDbConnection connection, ILogger<NotificationServices> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _collection = database.GetCollection<ConfigModel>(MongoCollection.Config);
            _logger = logger;
        }
        public ConfigModel FindOneByKey(string key)
        {
            try
            {
                return _collection.Find(x => x.Key == key).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
