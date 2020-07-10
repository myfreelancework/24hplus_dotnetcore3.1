
using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services.MC
{
    public class MCCheckCICService
    {
        private readonly ILogger<MCCheckCICService> _logger;
        // private readonly CustomerServices _customerServices;
        private readonly NotificationServices _notificationServices;
        private readonly IMongoCollection<MCCheckCICModel> _collection;
        public MCCheckCICService(IMongoDbConnection connection,
            ILogger<MCCheckCICService> logger,
            // CustomerServices customerServices,
            NotificationServices notificationServices)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _collection = database.GetCollection<MCCheckCICModel>(MongoCollection.MCCheckCIC);
            _logger = logger;
            // _customerServices = customerServices;
            _notificationServices = notificationServices;
        }

        public MCCheckCICModel CreateOne(MCCheckCICModel cICModel)
        {
            try
            {
                _collection.InsertOne(cICModel);
                return cICModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task ReplaceOneAsync(MCCheckCICModel cICModel)
        {
            try
            {
                await _collection.ReplaceOneAsync(c => c.Id == cICModel.Id, cICModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        
        public MCCheckCICModel FindOneByRequestId(string requestId)
        {
            try
            {
                return _collection.Find(x => x.RequestId == requestId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
