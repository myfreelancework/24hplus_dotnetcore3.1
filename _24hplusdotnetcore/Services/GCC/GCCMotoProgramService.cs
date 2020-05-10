using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Common.Constants;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Models.GCC;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace _24hplusdotnetcore.Services.GCC
{
    public class GCCMotoProgramService
    {
        private readonly ILogger<GCCMotoProgramService> _logger;
        private readonly IMongoCollection<GCCMotoProgramModel> _collection;
        private readonly IMongoCollection<GCCKindMotoModel> _motoCollection;
        public GCCMotoProgramService(IMongoDbConnection connection, ILogger<GCCMotoProgramService> logger)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _collection = database.GetCollection<GCCMotoProgramModel>(MongoCollection.GCCMotoProgram);
            _motoCollection = database.GetCollection<GCCKindMotoModel>(MongoCollection.GCCKindMoto);
            _logger = logger;
        }

        public GCCMotoProgramModel FindProgramByTitle(string title)
        {
            try
            {
                return _collection.Find(x => x.title == title).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }


        public GCCKindMotoModel FindMotoByName(string motoName)
        {
            try
            {
                return _motoCollection.Find(x => x.motoName == motoName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
    }
}
