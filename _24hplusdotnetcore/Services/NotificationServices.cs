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
    public class NotificationServices
    {
        private readonly ILogger<NotificationServices> _logger;
        private readonly IMongoCollection<Notification> _notification;
        public NotificationServices(IMongoDbConnection connection, ILogger<NotificationServices> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _notification = database.GetCollection<Notification>(MongoCollection.Notification);
            _logger = logger;
        }
        public List<Notification> GetAll(string UserName, int? pagenumber)
        {
            var lstCustomer = new List<Notification>();
            try
            {
                lstCustomer = _notification.Find(c => c.userName == UserName).SortByDescending(c => c.createAt).Skip((pagenumber != null && pagenumber > 0) ? ((pagenumber - 1) * Common.Config.PageSize) : 0).Limit(Common.Config.PageSize).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstCustomer;
        }
        public Notification FindOne(string id)
        {
            try
            {
                return _notification.Find(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public Notification CreateOne(Notification noti)
        {
            try
            {
                noti.createAt = Convert.ToDateTime(DateTime.Now.ToLocalTime());
                _notification.InsertOne(noti);
                return noti;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public long UpdateOne(Notification noti)
        {
            long updateCount = 0;
            try
            {
                updateCount = _notification.ReplaceOne(c => c.Id == noti.Id, noti).ModifiedCount;
            }
            catch (Exception ex)
            {
                updateCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }
        public long DeleteOne(string Id)
        {
            long DeleteCount = 0;
            try
            {
                DeleteCount = _notification.DeleteOne(c => c.Id == Id).DeletedCount;

            }
            catch (Exception ex)
            {
                DeleteCount = -1;
                _logger.LogError(ex, ex.Message);
            }
            return DeleteCount;
        }
    }
}
