using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _24hplusdotnetcore.Services
{
    public class ProductServices
    {
        private readonly ILogger<ProductServices> _logger;
        private readonly IMongoCollection<Product> _product;
        public ProductServices(ILogger<ProductServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _product = database.GetCollection<Product>(Common.MongoCollection.Product);
        }
        public List<Product> GetProducts()
        {
            var lstProduct = new List<Product>();
            try
            {
                lstProduct = _product.Find(p => true).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstProduct;
        }
        public Product GetProductByProductId(string ProductId)
        {
            var objProduct = new Product();
            try
            {
                objProduct = _product.Find(p => p.ProductId == ProductId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objProduct;
        }
        public List<Product> GetProductBygreen(string GreenType)
        {
            var lstProduct = new List<Product>();
            try
            {
                lstProduct = _product.Find(p => p.GreenType == GreenType).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstProduct;
        }
        public Product CreateProduct(Product product)
        {
            try
            {
                _product.InsertOne(product);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public List<Product> GetProductByProductCategory(string ProductCategoryId)
        {
            var lstProduct = new List<Product>();
            try
            {
                lstProduct = _product.Find(p => p.ProductCategoryId == ProductCategoryId).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstProduct;
        }
        public IEnumerable<Product> GetByIds(IEnumerable<string> ids)
        {
            try
            {
                return _product.Find(c => ids.Contains(c.Id)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
