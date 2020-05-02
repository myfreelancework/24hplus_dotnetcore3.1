using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace _24hplusdotnetcore.Services
{
    public class NotificationServices
    {
        private readonly ILogger<NotificationServices> _logger;
        private readonly IMongoCollection<Notification> _notification;
        private readonly UserLoginServices _userLoginservices;
        private readonly UserRoleServices _userRoleServices;
        private readonly IConfiguration _config;
        public NotificationServices(IMongoDbConnection connection, ILogger<NotificationServices> logger, UserLoginServices userLoginServices, UserRoleServices userRoleServices, IConfiguration config)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _notification = database.GetCollection<Notification>(MongoCollection.Notification);
            _logger = logger;
            _userLoginservices = userLoginServices;
            _userRoleServices = userRoleServices;
            _config = config;
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
                PushNotification(NotificationType.Add, noti.userName, noti.Id);
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
        private dynamic PushNotification(string notificationType, string userName, string notificationId)
        {
            var client = new RestClient("https://fcm.googleapis.com/fcm/send");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", " application/json");
            request.AddHeader("Authorization", " key="+_config["FireBase:ServerKey"] +"");
            string registrationToken = "";
            FireBaseNotification firebaseNoti = new FireBaseNotification();
            firebaseNoti.From = _config["FireBase:Sender"];
            firebaseNoti.CollapseKey = _config["FireBase:collapseKey"];

            if (notificationType == NotificationType.Add)
            {
                string teamLeadId = _userRoleServices.GetUserRoleByUserName(userName).TeamLead;
                registrationToken = _userLoginservices.Get(teamLeadId).registration_token;
            }
            else
            {
                registrationToken = _userLoginservices.Get(userName).registration_token;
            }
            var notification = FindOne(notificationId);
            firebaseNoti.RegistrationIds = new string[] {
                registrationToken
            };
            firebaseNoti.Notification = new NotificationFireBase
            {
                Body = notification.message
            };
                
            firebaseNoti.Data = new Data
            {
                NotificationId = new Random().Next(99999999),
                NotificationType = NotificationType.Add,
                GreenType = notification.green,
                RecordId = notification.Id,
                TotalNotifications = 1,
                Username = userName
            };           
            
            request.AddParameter(" application/json", ""+JsonConvert.SerializeObject(firebaseNoti) +"", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
    }
}
