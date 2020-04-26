using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class MobileVersionServices
    {
        private readonly ILogger<MobileVersionServices> _logger;
        private readonly IMongoCollection<MobileVersion> _mobileVersion;
        public MobileVersionServices(ILogger<MobileVersionServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _mobileVersion = database.GetCollection<MobileVersion>(Common.MongoCollection.MobileVersion);
        }
        public MobileVersion Create(MobileVersion mobileVersion)
        {
            try
            {
                _mobileVersion.InsertOne(mobileVersion);
                return mobileVersion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public List<MobileVersion> GetMobileVersions()
        {
            var lstMobileVersion = new List<MobileVersion>();
            try
            {
                lstMobileVersion = _mobileVersion.Find(m => true).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstMobileVersion;
        }
        public MobileVersion GetMobileVersion(string type, string version)
        {
            var objMobileVersion = new MobileVersion();
            try
            {
                objMobileVersion = _mobileVersion.Find(m => m.Type.ToLower() == type.ToLower() && m.Version == version).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objMobileVersion;
        }
    }
}
