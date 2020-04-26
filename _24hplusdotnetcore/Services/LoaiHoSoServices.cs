using System;
using System.Collections.Generic;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace _24hplusdotnetcore.Services
{
    public class DocumentCategoryServices
    {
        private readonly ILogger<DocumentCategoryServices> _logger;
        private readonly IMongoCollection<DocumentCategory> _loaiHS;
        public DocumentCategoryServices(ILogger<DocumentCategoryServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _loaiHS = database.GetCollection<DocumentCategory>(Common.MongoCollection.LoaiHoSo);
        }
        public List<DocumentCategory> GetList()
        {
            var lstLoaiHoSo = new List<DocumentCategory>();
            try
            {
                lstLoaiHoSo = _loaiHS.Find(h => true).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstLoaiHoSo;
        }
        public DocumentCategory GetDocumentCategory(string DocumentCategoryId)
        {
            var objLoaiHoSo = new DocumentCategory();
            try
            {
                objLoaiHoSo = _loaiHS.Find(h => h.DocumentCategoryId == DocumentCategoryId).FirstOrDefault();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objLoaiHoSo;
        }

        internal List<DocumentCategory> GetDocumentCategoryByGreenType(string greentId)
        {
            var lstLoaiHoSo = new List<DocumentCategory>();
            try
            {
                lstLoaiHoSo = _loaiHS.Find(h => h.GreenType == greentId).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstLoaiHoSo;
        }
    }
}