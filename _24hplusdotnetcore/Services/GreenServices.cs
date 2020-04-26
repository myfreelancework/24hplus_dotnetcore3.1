using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class GreenServices
    {
        private readonly ILogger<GreenServices> _logger;
        private readonly IMongoCollection<Green> _green;
        public GreenServices(ILogger<GreenServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _green = database.GetCollection<Green>(Common.MongoCollection.Green);
        }
        public List<Green> Getgreens()
        {
            var lstgreen = new List<Green>();
            try
            {
                lstgreen = _green.Find(p => true).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstgreen;
        }
        public Green Getgreen(string GreenType)
        {
            var objgreen = new Green();
            try
            {
                objgreen = _green.Find(p => p.GreenType == GreenType).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objgreen;
        }
        public Green Create(Green green)
        {
            try
            {
                _green.InsertOne(green);
                return green;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
