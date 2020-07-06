
using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.MC;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;

namespace _24hplusdotnetcore.Services.MC
{
    public class MCNotificationService
    {
        private readonly ILogger<MCNotificationService> _logger;
        private readonly CustomerServices _customerServices;
        private readonly NotificationServices _notificationServices;
        private readonly IMongoCollection<MCNotificationModel> _collection;
        public MCNotificationService(IMongoDbConnection connection,
            ILogger<MCNotificationService> logger,
            CustomerServices customerServices,
            NotificationServices notificationServices)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _collection = database.GetCollection<MCNotificationModel>(MongoCollection.MCNotification);
            _logger = logger;
            _customerServices = customerServices;
            _notificationServices = notificationServices;
        }

        public MCNotificationModel CreateOne(MCNotificationDto noti)
        {
            try
            {
                MCNotificationModel mcNoti = new MCNotificationModel();
                mcNoti.MCId = noti.Id;
                mcNoti.AppId = noti.AppId;
                mcNoti.AppNumber = noti.AppNumber;
                mcNoti.CurrentStatus = noti.CurrentStatus;
                _collection.InsertOne(mcNoti);
                return mcNoti;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
