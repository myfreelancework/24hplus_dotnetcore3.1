using System.Collections.Generic;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class DocumentCategoryController: ControllerBase
    {
        private readonly ILogger<DocumentCategoryController> _logger;
        private readonly Services.DocumentCategoryServices _loaiHSServices;
        public DocumentCategoryController(ILogger<DocumentCategoryController> logger, Services.DocumentCategoryServices loaiHS)
        {
            _loaiHSServices = loaiHS;
            _logger = logger;
        }
        [HttpGet]
        [Route("api/documentcategories")]
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
                var lstLoaiHS = new List<Models.DocumentCategory>();
                lstLoaiHS = _loaiHSServices.GetList();
                return Ok(new ResponseContext{
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstLoaiHS
                });
            }
            catch (System.Exception ex)
            {
               _logger.LogError(ex, ex.Message);
               return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage{
                   status = "ERROR",
                   message = ex.Message
               });
            }
        }
        [HttpGet]
        [Route("api/documentcategory/{DocumentCategoryId}")]
        public ActionResult<ResponseContext> GetLoaiHSByMaLoaiHS( string DocumentCategoryId)
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
                var objLoaiHS = new Models.DocumentCategory();
                objLoaiHS = _loaiHSServices.GetDocumentCategory(DocumentCategoryId);
                return Ok(new ResponseContext{
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = objLoaiHS
                });
            }
            catch (System.Exception ex)
            {
               _logger.LogError(ex, ex.Message);
               return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage{
                   status = "ERROR",
                   message = ex.Message
               });
            }
        }
        [HttpGet]
        [Route("api/documentcategory/green")]
        public ActionResult<ResponseContext> GetDocumentCategoryBygreen([FromQuery]string greentype)
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
                var lstLoaiHS = new List<DocumentCategory>();
                lstLoaiHS = _loaiHSServices.GetDocumentCategoryByGreenType(greentype);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstLoaiHS
                });
            }
            catch (System.Exception ex)
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