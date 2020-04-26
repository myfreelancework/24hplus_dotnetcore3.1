using System.Collections.Generic;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    
    public class UserController : ControllerBase
    {
        private readonly UserServices _userService;
        public static bool isLoggedByOrtherDevices;
        public UserController(UserServices userServices)
        {
            _userService = userServices;
        }
        
        [HttpPost]
        [Route("api/user")]        
        public ActionResult<User> Create(User user)
        {
            try
            {
                var newUser = _userService.Create(user);
                return Ok(newUser);
            }
            catch (System.Exception ex)
            {
                    return StatusCode(StatusCodes.Status500InternalServerError ,new ResponseMessage{status = "ERROR", message = ex.Message});
            }
        }

        [HttpGet]
        [Route("api/users")]
        public ActionResult<ResponseContext> Get()
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
                var lstUser = _userService.Get();
                return Ok(new ResponseContext{
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstUser
                });
            }
            catch (System.Exception ex)
            {
                
                 return StatusCode(StatusCodes.Status500InternalServerError ,new ResponseMessage{status = "ERROR", message = ex.Message});
            }
        }

        [HttpGet]
        [Route("api/user/{userName}")]
        public ActionResult<ResponseContext> Get(string userName)
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
                var objUser = _userService.Get(userName);
                return Ok(new ResponseContext{
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = objUser
                });
            }
            catch (System.Exception ex)
            {
                 return StatusCode(StatusCodes.Status500InternalServerError ,new ResponseMessage{status = "ERROR", message = ex.Message});
            }
            
        }

    }
}