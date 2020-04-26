using _24hplusdotnetcore.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using _24hplusdotnetcore.Common;

namespace _24hplusdotnetcore.Services
{
    public class DemoService
    {
        private readonly IMongoCollection<Demo> _demo;
        public DemoService(IMongoDbConnection connection)
        {
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _demo = database.GetCollection<Demo>(MongoCollection.DemoCollection);
        }
        public List<Demo> Get() => _demo.Find(demo => true).ToList();
        public Demo Get(string id) => _demo.Find(d => d.Id == id).FirstOrDefault();
        public Demo Create(Demo demo)
        {
            _demo.InsertOne(demo);
            return demo;
        }
        public void Update(string id, Demo demoIn) => 
            _demo.ReplaceOne(d => d.Id == id, demoIn);
        public void Delete(Demo demoIn)
        {
            _demo.DeleteOne(d => d.Id == demoIn.Id);
        }
         public void Delete(string id)
        {
            _demo.DeleteOne(d => d.Id == id);
        }
    }
}