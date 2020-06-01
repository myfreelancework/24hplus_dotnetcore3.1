using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.CRM;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;

namespace _24hplusdotnetcore.Services.CRM
{
    public class DataCRMProcessingServices
    {
        private readonly ILogger<DataCRMProcessingServices> _logger;
        private readonly IMongoCollection<DataCRMProcessing> _dataCRMProcessing;
        public DataCRMProcessingServices(ILogger<DataCRMProcessingServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _dataCRMProcessing = database.GetCollection<DataCRMProcessing>(Common.MongoCollection.DataCRMProcessing);
        }

        public DataCRMProcessing CreateOne(DataCRMProcessing dataCRM)
        {
            var newData = new DataCRMProcessing();
            try
            {
                _dataCRMProcessing.InsertOne(dataCRM);
                newData = dataCRM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return newData;
        }

        public long UpdateByCustomerId(string CustomerId, string Status)
        {
            return 1;
        }
    }
}
