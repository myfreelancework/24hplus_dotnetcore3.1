using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using AutoMapper.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class LeadCrmService
    {
        private readonly ILogger<LeadCrmService> _logger;
        private readonly IMongoCollection<LeadCrm> _leadCrmCollection;

        public LeadCrmService(
            ILogger<LeadCrmService> logger,
            IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _leadCrmCollection = database.GetCollection<LeadCrm>(MongoCollection.LEAD_CRM);
        }

        public async Task<IEnumerable<LeadCrm>> GetByLeadCrmIdsAsync(IEnumerable<string> leadCrmIds)
        {
            return await _leadCrmCollection.Find(c => leadCrmIds.Contains(c.LeadCrmId)).ToListAsync();
        }

        public async Task InsertAsync(LeadCrm leadCrm)
        {
            await _leadCrmCollection.InsertOneAsync(leadCrm);
        }

        public async Task ReplaceOneAsync(LeadCrm leadCrm)
        {
            await _leadCrmCollection.ReplaceOneAsync(c => c.Id == leadCrm.Id, leadCrm);
        }

        public IEnumerable<LeadCrm> GetByIds(IEnumerable<string> ids)
        {
            return _leadCrmCollection.Find(c => ids.Contains(c.Id)).ToList();
        }

        public async Task<LeadCrm> GetByPotentialNoAsync(string potentialNo)
        {
            return await _leadCrmCollection.Find(c => c.PotentialNo == potentialNo).FirstOrDefaultAsync();
        }
    }
}
