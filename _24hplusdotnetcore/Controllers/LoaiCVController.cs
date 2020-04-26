using System;
using System.Collections.Generic;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class JobCategoryController : ControllerBase
    {
        private readonly ILogger<JobCategoryController> _logger;
        private readonly LoaiCVServices _loaiCVServices;
        public JobCategoryController(ILogger<JobCategoryController> logger, LoaiCVServices loaiCVServices)
        {
            _logger = logger;
            _loaiCVServices = loaiCVServices;
        }
        [HttpGet]
        [Route("api/jobcategories")]
        public ActionResult<ResponseContext> GetList()
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
                var lstLoaicv = new List<JobCategory>();
                lstLoaicv = _loaiCVServices.GetList();
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstLoaicv
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/jobcategory/{JobCategoryId}")]
        public ActionResult<ResponseContext> GetLoaiCV(string JobCategoryId)
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
                var objLoaiCV = new JobCategory();
                objLoaiCV = _loaiCVServices.GetJobCategoryByCategoryId(JobCategoryId);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = objLoaiCV
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }
        [HttpGet]
        [Route("api/jobcategory")]
        public ActionResult<ResponseContext> GetJobCategoryByGreenType([FromQuery] string greentype)
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
                var lstLoaiCV = new List<JobCategory>();
                lstLoaiCV = _loaiCVServices.GetJobCategoryByGreenType(greentype);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstLoaiCV
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }
    }
}