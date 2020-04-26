using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class PaymentServices
    {
        private readonly ILogger<PaymentServices> _logger;
        private readonly IMongoCollection<Payment> _payment;
        public PaymentServices(ILogger<PaymentServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _payment = database.GetCollection<Payment>(Common.MongoCollection.Payment); 
        }
        public List<Payment> GetPayments(int? pagenumber, int? pagesize, ref int totalPage)
        {
            var lstPayment = new List<Payment>();
            try
            {
                int _pagesize = !pagesize.HasValue ? Common.Config.PageSize : (int)pagesize;
                lstPayment = _payment.Find(p => true).Skip((pagenumber != null && pagenumber > 0) ? ((pagenumber - 1) * _pagesize) : 0).Limit(_pagesize).ToList();
                var lstCount = _payment.Find(p => true).ToList().Count;
                if (lstCount == 0)
                {
                    totalPage = 0;
                }
                else
                {
                    if (lstCount <= _pagesize)
                    {
                        totalPage = 1;
                    }
                    else
                    {
                        totalPage = lstCount / _pagesize + lstCount % _pagesize;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstPayment;
        }
    }
}
