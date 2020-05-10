using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using _24hplusdotnetcore.Common;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly FileUploadServices _fileUploadServices;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public FileUploadController(ILogger<FileUploadController> logger, FileUploadServices fileUploadServices, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _fileUploadServices = fileUploadServices;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        [Route("api/fileuploads/{CustomerId}")]
        public ActionResult<ResponseContext> GetFileUploadsById(string CustomerId)
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
                var lstFileUpload = new List<FileUpload>();
                lstFileUpload = _fileUploadServices.GetListFileUploadByCustomerId(CustomerId);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstFileUpload
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("api/fileupload/create")]
        public ActionResult<ResponseContext> Create(FileUpload fileUpload)
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
                var newFileUpload = _fileUploadServices.CreateFileUpload(fileUpload);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = newFileUpload
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }
        [HttpPost]
        [Route("api/fileupload/update")]
        public ActionResult<ResponseContext> Update([FromBody] FileUpload[] fileUploads)
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
                var updateCount = _fileUploadServices.UpdateFileUpLoad(fileUploads);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = JsonConvert.SerializeObject(""+ updateCount + " records have been updated")
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
        [HttpPost]
        [Route("api/fileupload/delete")]
        public ActionResult<ResponseContext> FileUploadDelete([FromBody] string[] FileUploadId)
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
                var deleteCount = _fileUploadServices.DeleteFileUpload(FileUploadId);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = JsonConvert.SerializeObject("" + deleteCount + " records have been delete")
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
        [HttpPost]
        [Route("api/fileupload/upload")]
        public async Task<ActionResult> UploadAsync([FromForm(Name = "")] IFormFile file, [FromForm] string DocumentCategoryId, [FromForm] string CustomerId)
        {
            try
            {
                string serverPath = Path.Combine(_hostingEnvironment.ContentRootPath, "FileUpload");
                if (!Directory.Exists(serverPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(serverPath);
                }
                string path = Path.Combine(serverPath, file.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var fileUpload = new FileUpload
                {
                    CustomerId = CustomerId,
                    DocumentCategoryId = DocumentCategoryId,
                    FileUploadName = file.FileName,
                    FileUploadURL = string.Format(@"{0}://{1}/{2}/{3}/{4}", Request.Scheme, Request.Host.Value, CustomerId, DocumentCategoryId, file.FileName)
                };

                var newFileUpload = _fileUploadServices.CreateFileUpload(fileUpload);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = newFileUpload
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