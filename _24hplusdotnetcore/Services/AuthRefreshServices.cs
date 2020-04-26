using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class AuthRefreshServices
    {
        private readonly ILogger<AuthRefreshServices> _logger;
        private readonly IMongoCollection<AuthRefresh> _authRefresh;
        private readonly IConfiguration _config;
        public AuthRefreshServices(IMongoDbConnection connection, IConfiguration config, ILogger<AuthRefreshServices> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _authRefresh = database.GetCollection<AuthRefresh>(MongoCollection.AuthRefresh);
            //_dataProtectionProvider = dataProtectionProvider;
            _config = config;
            _logger = logger;
        }
        public string GetUserNameByRefreshToken(string refreshToken)
        {
            string userName = "";
            try
            {
                var authRefresh = _authRefresh.Find(a => a.RefresToken == refreshToken).FirstOrDefault();
                if (authRefresh != null)
                {
                    userName = authRefresh.UserName;
                }
;            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return userName;
        }
        public AuthRefresh CreateAuthRefresh(AuthRefresh authRefresh)
        {
            try
            {
                _authRefresh.InsertOne(authRefresh);
                return authRefresh;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public long UpdateRefreshToken(AuthRefresh authRefresh)
        {
            long updateCount = 0;
            try
            {
                updateCount = _authRefresh.ReplaceOne(i => i.UserName == authRefresh.UserName, authRefresh).ModifiedCount;  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }
        public long DeleteRefreshToken(string UserName)
        {
            long deleteCount = 0;
            try
            {
                deleteCount = _authRefresh.DeleteOne(a => a.UserName == UserName).DeletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return deleteCount;
        }
    }
}
