using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _24hplusdotnetcore.Services
{
    public class FileUploadServices
    {
        private readonly ILogger<FileUploadServices> _logger;
        private readonly IMongoCollection<FileUpload> _fileUpload;
        public FileUploadServices(ILogger<FileUploadServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _fileUpload = database.GetCollection<FileUpload>(Common.MongoCollection.FileUpload);
        }
        public List<FileUpload> GetListFileUploadByCustomerId(string CustomerId)
        {
            var lstFileUpload = new List<FileUpload>();
            try
            {
                lstFileUpload = _fileUpload.Find(f => f.CustomerId == CustomerId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstFileUpload;
        }
        public FileUpload GetFileUploadByFileUploadId(string fileUpaloadId)
        {
            var objFileUpload = new FileUpload();
            try
            {
                objFileUpload = _fileUpload.Find(f => f.FileUploadId == fileUpaloadId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objFileUpload;
        }
        public FileUpload CreateFileUpload(FileUpload fileUpload)
        {
            try
            {
                _fileUpload.InsertOne(fileUpload);
                return fileUpload;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public long UpdateFileUpLoad(FileUpload[] fileUpload)
        {
            long updateCount = 0;
            try
            {
                for (int i = 0; i < fileUpload.Length; i++)
                {
                    updateCount += _fileUpload.ReplaceOne(f => f.FileUploadId == fileUpload[i].FileUploadId, fileUpload[i]).ModifiedCount;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }
        public long DeleteFileUpload(string[] FileUploadId)
        {
            long deleteCount = 0;
            try
            {
                for (int i = 0; i < FileUploadId.Length; i++)
                {
                    deleteCount += _fileUpload.DeleteOne(f => f.FileUploadId == FileUploadId[i]).DeletedCount;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return deleteCount;
        }
    }
}
