using System;
using System.Collections.Generic;
using System.Dynamic;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]

    public class NotificationController : ControllerBase
    {
        private readonly ILogger<NotificationController> _logger;
        private readonly NotificationServices _notificationServices;

        public NotificationController(ILogger<NotificationController> logger, NotificationServices notificationServices)
        {
            _logger = logger;
            _notificationServices = notificationServices;
        }

        [HttpGet]
        [Route("api/notification/getall")]
        public ActionResult<ResponseContext> GetAllNotifications([FromQuery] string username, [FromQuery] int? pagenumber)
        {
            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = "Thiếu username",
                        data = null
                    });
                }
                if ((bool)HttpContext.Items["isLoggedInOtherDevice"])
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.IS_LOGGED_IN_ORTHER_DEVICE,
                        message = Common.Message.IS_LOGGED_IN_ORTHER_DEVICE,
                        data = null
                    });
                }
                var listItems = new List<Notification>();
                listItems = _notificationServices.GetAll(username, pagenumber);

                return Ok(new PagingDataResponse
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = listItems,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
        [HttpGet]
        [Route("api/notification/update")]
        public ActionResult<ResponseContext> Update([FromQuery] string Id, [FromQuery] bool isRead)
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
                var noti = new Notification();
                noti = _notificationServices.FindOne(Id);
                noti.isRead = isRead;
                var result = _notificationServices.UpdateOne(noti);

                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = result
                });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
        
        [HttpPost]
        [Route("api/notification/create")]
        public ActionResult<ResponseContext> Create(Notification noti)
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
                var newNoti = new Notification();
                newNoti = _notificationServices.CreateOne(noti);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = newNoti
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