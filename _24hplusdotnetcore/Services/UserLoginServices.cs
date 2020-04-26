using System;
using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace _24hplusdotnetcore.Services
{
    public class UserLoginServices
    {
        private readonly ILogger<UserLoginServices> _logger;
        private readonly IMongoCollection<UserLogin> _userLogin;
        private readonly IMongoCollection<User> _user;
        public UserLoginServices(ILogger<UserLoginServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _userLogin = database.GetCollection<UserLogin>(MongoCollection.UserLogin);
            _user = database.GetCollection<User>(MongoCollection.UsersCollection);
        }
        public UserLogin Create(UserLogin userLogin)
        {
            try
            {
                _userLogin.InsertOne(userLogin);
                return userLogin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public long Update(string LoginId, UserLogin userLogin)
        {
            long updateCount = 0;
            try
            {
                updateCount = _userLogin.ReplaceOne(u => u.LoginId == userLogin.LoginId, userLogin).ModifiedCount;
            }
            catch (Exception ex)
            {
                updateCount = -1;
                _logger.LogError(ex, ex.Message);                
            }
            return updateCount;
        }
        public long Delete(string userName)
        {
            long deleteCount = 0;
            try
            {
                deleteCount = _userLogin.DeleteOne(u => u.UserName == userName).DeletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return deleteCount;
        }
        public UserLogin Get(string userName)
        {
            var userLogin = new UserLogin();
            try
            {
                userLogin = _userLogin.Find(u => u.UserName == userName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return userLogin;
        }
        public UserLogin GetUserLoginByToken(string token)
        {
            var userLogin = new UserLogin();
            try
            {
                userLogin = _userLogin.Find(u => u.token == token).FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                 _logger.LogError(ex, ex.Message);
            }
            return userLogin;
        }
    }
}