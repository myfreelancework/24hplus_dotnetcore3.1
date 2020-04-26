using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    
    public class ProductCategoryController : ControllerBase
    {
        private readonly ILogger<ProductCategoryController> _logger;
        private readonly ProductCategoryServices _productCategoryServices;
        public ProductCategoryController(ILogger<ProductCategoryController> logger, ProductCategoryServices productCategoryServices)
        {
            _logger = logger;
            _productCategoryServices = productCategoryServices;
        }
        [HttpGet]
        [Route("api/productcategories")]
        public ActionResult<ResponseContext> Get()
        {
            var lstProductCategory = new List<ProductCategory>();
            try
            {
                if ((bool)HttpContext.Items["isLoggedInOtherDevice"])
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.IS_LOGGED_IN_ORTHER_DEVICE,
                        message = Common.Message.IS_LOGGED_IN_ORTHER_DEVICE,
                        data = null
                    });
                lstProductCategory = _productCategoryServices.GetProductCategories();
                return Ok(lstProductCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR " + StatusCodes.Status500InternalServerError, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("api/productcategory/{ProductCategoryId}")]
        public ActionResult<ResponseContext> Get(string ProductCategoryId)
        {
            dynamic objProductCategory;
            try
            {
                if ((bool)HttpContext.Items["isLoggedInOtherDevice"])
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.IS_LOGGED_IN_ORTHER_DEVICE,
                        message = Common.Message.IS_LOGGED_IN_ORTHER_DEVICE,
                        data = null
                    });
                objProductCategory = _productCategoryServices.GetProductCategory(ProductCategoryId);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = objProductCategory
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR " + StatusCodes.Status500InternalServerError, message = ex.Message });
            }
        }
        [HttpGet]
        [Route("api/productcategory")]
        public ActionResult<ResponseContext> GetProdctCategoryByGreenType([FromQuery]string greentype)
        {
            dynamic lstProductCategory = new List<dynamic>();
            try
            {
                if ((bool)HttpContext.Items["isLoggedInOtherDevice"])
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.IS_LOGGED_IN_ORTHER_DEVICE,
                        message = Common.Message.IS_LOGGED_IN_ORTHER_DEVICE,
                        data = null
                    });
                lstProductCategory = _productCategoryServices.GetProductCategoryBygreen(greentype);
                return Ok(new ResponseContext { 
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstProductCategory
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR " + StatusCodes.Status500InternalServerError, message = ex.Message });
            }
        }
        [HttpPost]
        [Route("api/productcategory")]
        public ActionResult<ResponseContext> Create(ProductCategory productCategory)
        {
            try
            {
                if ((bool)HttpContext.Items["isLoggedInOtherDevice"])
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.IS_LOGGED_IN_ORTHER_DEVICE,
                        message = Common.Message.IS_LOGGED_IN_ORTHER_DEVICE,
                        data = null
                    });
                _productCategoryServices.Create(productCategory);
                return Ok(productCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR " + StatusCodes.Status500InternalServerError, message = ex.Message });
            }
        }
    }
}