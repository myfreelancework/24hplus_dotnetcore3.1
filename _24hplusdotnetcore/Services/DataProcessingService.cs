using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _24hplusdotnetcore.Services
{
    public class DataProcessingService
    {
        private readonly ILogger<DataProcessingService> _logger;
        private readonly IMongoCollection<DataProcessing> _dataProcessing;

        public DataProcessingService(
            IMongoDbConnection connection,
            ILogger<DataProcessingService> logger
            )
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _dataProcessing = database.GetCollection<DataProcessing>(MongoCollection.DATA_PROCESSING);
        }

        public DataProcessing ReplaceOne(DataProcessing dataProcessing)
        {
            try
            {
                var filter = Builders<DataProcessing>.Filter.Where(c =>
                string.Equals(c.CustomerId, dataProcessing.CustomerId) && string.Equals(c.Status, dataProcessing.Status));

                _dataProcessing.ReplaceOne(filter, dataProcessing, new ReplaceOptions { IsUpsert = true });
                return dataProcessing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public void DeleteByIds(IEnumerable<string> ids)
        {
            try
            {
                _dataProcessing.DeleteMany(x => ids.Contains(x.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IEnumerable<DataProcessing> GetList(string dataProcessingType, string status)
        {
            try
            {
                return _dataProcessing.Find(x => string.Equals(dataProcessingType, x.DataProcessingType) && 
                string.Equals(x.Status, status)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
