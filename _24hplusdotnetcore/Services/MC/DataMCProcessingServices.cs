using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MC
{
    public class DataMCProcessingServices
    {
        private readonly ILogger<DataMCProcessingServices> _logger;
        private readonly IMongoCollection<DataMCProcessing> _dataMCProcessing;
        public DataMCProcessingServices(ILogger<DataMCProcessingServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _dataMCProcessing = database.GetCollection<DataMCProcessing>(Common.MongoCollection.DataMCProcessing);
        }
        public DataMCProcessing CreateOne(DataMCProcessing dataMC)
        {
            var newData = new DataMCProcessing();
            try
            {
                _dataMCProcessing.InsertOne(dataMC);
                newData = dataMC;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return newData;
        }
        public long UpdateByCustomerId(string CustomerId, string Status)
        {
            try
            {
                var dataMC = _dataMCProcessing.Find(d => d.CustomerId == CustomerId).FirstOrDefault();
                dataMC.Status = Status;
                var modifiedCount = _dataMCProcessing.ReplaceOne(d => d.Id == dataMC.Id, dataMC).ModifiedCount;
                return modifiedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return -1;
            }
        }

        public List<DataMCProcessing> GetDataMCProcessings(string status)
        {
            var listDataCRMProcessing = new List<DataMCProcessing>();
            try
            {
                listDataCRMProcessing = _dataMCProcessing.Find(d => d.Status == status).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return listDataCRMProcessing;
        }
    }
}
