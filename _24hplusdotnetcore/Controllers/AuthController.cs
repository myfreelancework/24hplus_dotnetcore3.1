using System;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly AuthServices _authServices;
        private readonly AuthRefreshServices _authRefreshServices;
        private readonly UserLoginServices _userLoginServices;
        public AuthController(AuthServices authServices, AuthRefreshServices authRefreshServices, UserLoginServices userLoginServices, ILogger<AuthController> logger)
        {
            _logger = logger;
            _authServices = authServices;
            _authRefreshServices = authRefreshServices;
            _userLoginServices = userLoginServices;
        }


        [HttpPost]
        [Route("api/auth/login")]
        public ActionResult Login(User user)
        {
            try
            {
                var authInfo = new AuthInfo();
                authInfo = _authServices.Login(user);
                if (authInfo != null)
                {
                    var authRefresh = new AuthRefresh();
                    authRefresh.UserName = authInfo.UserName;
                    authRefresh.RefresToken = authInfo.RefreshToken;
                    var newAuthRefresh = new AuthRefresh();
                    newAuthRefresh = _authRefreshServices.CreateAuthRefresh(authRefresh);
                    if (newAuthRefresh != null)
                    {
                        return Ok(authInfo);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new { message = Common.Message.LOGIN_BIDDEN });
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new { message = Common.Message.INCORRECT_USERNAME_PASSWORD });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }

        }
        [HttpPost]
        [Route("api/auth/userlogin")]
        public ActionResult Login(RequestLoginInfo requestLoginInfo)
        {
            try
            {
                var resLogin = new ResponseLoginInfo();
                resLogin = _authServices.LoginWithoutRefeshToken(requestLoginInfo);
                if (resLogin != null && resLogin.UserName != null)
                {
                    var prevUserLogin = new UserLogin();
                    prevUserLogin = _userLoginServices.Get(requestLoginInfo.UserName);
                    if (prevUserLogin == null || prevUserLogin.UserName == null)
                    {
                        var newUserLogin = new UserLogin
                        {
                            LoginId = "",
                            UserName = requestLoginInfo.UserName,
                            uuid = requestLoginInfo.uuid,
                            ostype = requestLoginInfo.ostype,
                            token = resLogin.token
                        };
                        var createdUser = _userLoginServices.Create(newUserLogin);
                        if (createdUser != null)
                        {
                            return Ok(new ResponseContext
                            {
                                code = (int)Common.ResponseCode.SUCCESS,
                                message = Common.Message.LOGIN_SUCCESS,
                                data = resLogin
                            });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                            {
                                status = "ERROR",
                                message = "Cannot update user login information"
                            });
                        }
                    }
                    else
                    {
                        var updateUserLogin = new UserLogin
                        {
                            LoginId = prevUserLogin.LoginId,
                            UserName = requestLoginInfo.UserName,
                            uuid = requestLoginInfo.uuid,
                            ostype = requestLoginInfo.ostype,
                            token = resLogin.token
                        };
                        var updateCount = _userLoginServices.Update(prevUserLogin.LoginId, updateUserLogin);
                        if (updateCount >= 0)
                        {
                            return Ok(new ResponseContext
                            {
                                code = (int)Common.ResponseCode.SUCCESS,
                                message = Common.Message.LOGIN_SUCCESS,
                                data = resLogin
                            });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage
                            {
                                status = "ERROR",
                                message = "Cannot update user login information"
                            });
                        }
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = Common.Message.INCORRECT_USERNAME_PASSWORD,
                        data = null
                    });
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
    }
}