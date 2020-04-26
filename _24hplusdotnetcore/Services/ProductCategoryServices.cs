using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace _24hplusdotnetcore.Services
{
    public class ProductCategoryServices
    {
        private readonly ILogger<ProductCategoryServices> _logger;
        private readonly IMongoCollection<ProductCategory> _productCategory;
        private readonly IMongoCollection<Product> _product;
        public ProductCategoryServices(ILogger<ProductCategoryServices> logger, IMongoDbConnection connection)
        {
            _logger = logger;
            var client = new MongoClient(connection.ConnectionString);
            var database = client.GetDatabase(connection.DataBase);
            _productCategory = database.GetCollection<ProductCategory>(MongoCollection.ProductCategoryCollection);
            _product = database.GetCollection<Product>(MongoCollection.Product);
        }
        public List<ProductCategory> GetProductCategories()
        {
            var lstProductCategories = new List<ProductCategory>();
            try
            {
                lstProductCategories = _productCategory.Find(p => true).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstProductCategories;
        }
        public dynamic GetProductCategory(string ProductCategoryId)
        {
            dynamic objProductDetails = new ExpandoObject();
            try
            {
                var objProductCategory = _productCategory.Find(p => p.ProductCategoryId == ProductCategoryId).FirstOrDefault();
                if (objProductCategory != null)
                {
                    
                    objProductDetails.Id = objProductCategory.Id;
                    objProductDetails.ProductCategoryId = objProductCategory.ProductCategoryId;
                    objProductDetails.ProductCategoryName = objProductCategory.ProductCategoryName;
                    objProductDetails.GreenType = objProductCategory.GreenType;
                    objProductDetails.Note = objProductCategory.Note;
                    var lstProducts = new List<Product>();
                    lstProducts = _product.Find(p => p.ProductCategoryId == objProductCategory.ProductCategoryId).ToList();
                    objProductDetails.Products = lstProducts;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return objProductDetails;
        }
        public List<dynamic> GetProductCategoryBygreen(string greentId)
        {

            var lstProductCategory = new List<ProductCategory>();
            var lstProductDetails = new List<dynamic>();
            try
            {
                lstProductCategory = _productCategory.Find(p => p.GreenType == greentId).ToList();
               
                for (int i = 0; i < lstProductCategory.Count; i++)
                {
                    dynamic objProductDetails = new ExpandoObject();
                    var lstProduct = _product.Find(p => p.ProductCategoryId == lstProductCategory[i].ProductCategoryId).ToList();
                    objProductDetails.Id = lstProductCategory[i].Id;
                    objProductDetails.ProductCategoryId = lstProductCategory[i].ProductCategoryId;
                    objProductDetails.ProductCategoryName = lstProductCategory[i].ProductCategoryName;
                    objProductDetails.GreenType = lstProductCategory[i].GreenType;
                    objProductDetails.Note = lstProductCategory[i].Note;
                    objProductDetails.Products = lstProduct;
                    lstProductDetails.Add(objProductDetails);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return lstProductDetails;
        }
        public ProductCategory Create(ProductCategory productCategory)
        {
            try
            {
                _productCategory.InsertOne(productCategory);
                return productCategory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public long Update(ProductCategory productCategory)
        {
            long updateCount = 0;
            try
            {
                updateCount = _productCategory.ReplaceOne(r => r.Id == productCategory.Id, productCategory).ModifiedCount;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, ex.Message);
            }
            return updateCount;
        }
        public long Detete(string Id)
        {
            long deleteCount = 0;
            try
            {
                deleteCount = _productCategory.DeleteOne(p => p.Id == Id).DeletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return deleteCount;
        }

    }
}
