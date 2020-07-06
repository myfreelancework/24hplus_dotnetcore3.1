using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.CRM;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

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

        public long UpdateByCustomerId(DataCRMProcessing dataCRMProcessing, string Status)
        {
            try
            {
                dataCRMProcessing.Status = Status;
                var modifiedCount = _dataCRMProcessing.ReplaceOne(d => d.Id == dataCRMProcessing.Id, dataCRMProcessing).ModifiedCount;
                return modifiedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return -1;
            }
        }

        public List<DataCRMProcessing> GetDataCRMProcessings(string status)
        {
            var listDataCRMProcessing = new List<DataCRMProcessing>();
            try
            {
                listDataCRMProcessing = _dataCRMProcessing.Find(d => d.Status == status).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return listDataCRMProcessing;
        }
    }
}
