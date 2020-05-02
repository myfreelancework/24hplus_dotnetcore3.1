using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Builder;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using _24hplusdotnetcore.Models;
using System.Runtime.Serialization;

namespace _24hplusdotnetcore.Middleware
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly UserLoginServices _userLoginSerivces;

        public RequestMiddleware(RequestDelegate next, UserLoginServices userLoginSerivces)
        {
            _next = next;
            _userLoginSerivces = userLoginSerivces;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers["Authorization"].Count > 0)
            {
                var auth = context.Request.Headers["Authorization"][0];
                var authArray = auth.Split(" ");
                var token = authArray[1];
                var userlogin = _userLoginSerivces.GetUserLoginByToken(token);
                if (userlogin == null || userlogin.UserName == null)
                {
                    context.Items["isLoggedInOtherDevice"] = true;
                }
                else
                {
                    context.Items["isLoggedInOtherDevice"] = false;
                }
                await _next(context);

            }
            else
            {
                if (!context.Request.Path.Value.Contains("api/auth/userlogin")
                 && !context.Request.Path.Value.Contains("swagger")
                 && !context.Request.Path.Value.Contains("api/checkversion")
                 && !context.Request.Path.Value.Contains("api/config/banner")
                 && !context.Request.Path.Value.Contains("api/gcc/personal"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Headers.Clear();

                }
                else
                {
                    await _next(context);
                }
            }
            // Call the next delegate/middleware in the pipeline

        }
    }
    public static class RequestAPIMiddlewareExtensions
    {
        public static IApplicationBuilder RequestAPIMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestMiddleware>();
        }
    }
}