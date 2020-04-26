using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _24hplusdotnetcore.Controllers
{
    //[ApiController]
    public class DemoController : ControllerBase
    {
        private readonly DemoService _demoService;
        public DemoController(DemoService demoService)
        {
            _demoService = demoService;
        }

    //     [HttpGet]
    //     [Route("api/demos")]
    //     public List<Demo> Get() => _demoService.Get();

    //     [HttpGet]
    //     [Route("api/demo/{id}")]
    //     public Demo GetDemoById(string id) => _demoService.Get(id);

    //     [HttpPost]
    //     [Route("api/demo")]
    //     public ActionResult<Demo> Create(Demo demo)
    //     {
    //         try
    //         {
    //             var newdemo = _demoService.Create(demo);
    //             return Ok(newdemo);
    //         }
    //         catch (Exception ex)
    //         {
                
    //             return StatusCode(StatusCodes.Status500InternalServerError ,new ResponseMessage{status = "ERROR", message = ex.Message});
    //         }
            
    //     }
    }
} 