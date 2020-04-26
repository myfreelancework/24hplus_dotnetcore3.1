using System.Collections.Generic;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace _24hplusdotnetcore.Services
{
    public class LoaiCVServices
    {
        private readonly ILogger<DocumentCategoryServices> _logger;
        private readonly IMongoCollection<JobCategory> _loaiCV;
        public LoaiCVServices(ILogger<DocumentCategoryServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _loaiCV = database.GetCollection<JobCategory>(Common.MongoCollection.LoaiCV);
        }
        public List<JobCategory> GetList()
        {
            var lstLoaiCV = new List<JobCategory>();
            try
            {
                lstLoaiCV = _loaiCV.Find(l => true).ToList();
            }
            catch (System.Exception ex)
            {
                 _logger.LogError(ex, ex.Message);
            }
            return lstLoaiCV;
        }
        public JobCategory GetJobCategoryByCategoryId(string JobCategoryId)
        {
            var objLoaiCV = new JobCategory();
            try
            {
                objLoaiCV = _loaiCV.Find(c => c.JobCategoryId == JobCategoryId).FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objLoaiCV;
        }
        public List<JobCategory> GetJobCategoryByGreenType(string GreenType)
        {
            var lstLoaiCV = new List<JobCategory>();
            try
            {
                lstLoaiCV = _loaiCV.Find(l => l.GreenType == GreenType).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstLoaiCV;
        }
    }
}
