using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task ReplaceOneAsync(DataProcessing dataProcessing)
        {
            try
            {
                await _dataProcessing.ReplaceOneAsync(c => c.Id == dataProcessing.Id, dataProcessing);
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

        public async Task<IEnumerable<DataProcessing>> GetListAsync(string dataProcessingType, string status)
        {
            try
            {
                return await _dataProcessing.Find(x => string.Equals(dataProcessingType, x.DataProcessingType) && 
                string.Equals(x.Status, status)).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public IEnumerable<DataProcessing> InsertMany(IEnumerable<DataProcessing> dataProcessings)
        {
            _dataProcessing.InsertMany(dataProcessings);
            return dataProcessings;
        }
    }
}
