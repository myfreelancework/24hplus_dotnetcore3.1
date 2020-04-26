using _24hplusdotnetcore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _24hplusdotnetcore.Controllers
{
    [ApiController]
    public class UnauthorizedController: ControllerBase
    {
        public UnauthorizedController()
        {

        }
        [HttpGet]
        [Route("api/auth/unauthorized")]
        public ActionResult unauthorized()
        {
             return StatusCode(StatusCodes.Status401Unauthorized, new ResponseContext{
                 code = (int)Common.ResponseCode.UNAUTHORIZED,
                 message = Common.Message.IS_LOGGED_IN_ORTHER_DEVICE,
                 data = null
             });
        }
    }
}