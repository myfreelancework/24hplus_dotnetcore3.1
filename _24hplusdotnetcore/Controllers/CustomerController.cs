﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using _24hplusdotnetcore.ModelDtos;
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

    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomerServices _customerServices;
        private readonly FileUploadServices _fileUploadServices;

        public CustomerController(ILogger<CustomerController> logger, CustomerServices customerServices, FileUploadServices fileUploadServices)
        {
            _logger = logger;
            _customerServices = customerServices;
            _fileUploadServices = fileUploadServices;
        }

        [HttpGet]
        [Route("api/customers")]
        public ActionResult<ResponseContext> GetCustomerList([FromQuery] string username, [FromQuery] DateTime? datefrom, [FromQuery] DateTime? dateto, [FromQuery] string status, [FromQuery] string customername, [FromQuery] string greentype, [FromQuery] int? pagenumber, [FromQuery] int? pagesize)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(greentype))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = "Thiếu username hoặc greentype",
                        data = null
                    });
                }
                if ((bool)HttpContext.Items["isLoggedInOtherDevice"])
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.IS_LOGGED_IN_ORTHER_DEVICE,
                        message = Common.Message.IS_LOGGED_IN_ORTHER_DEVICE,
                        data = null
                    });
                var lstCustomers = new List<Customer>();
                int totalPage = 0;
                int totalrecord = 0;
                lstCustomers = _customerServices.GetList(username, datefrom, dateto, status, greentype, customername, pagenumber, pagesize, ref totalPage, ref totalrecord);

                var lstCustomerOptimization = new List<dynamic>();

                for (int i = 0; i < lstCustomers.Count; i++)
                {
                    dynamic item = new ExpandoObject();
                    item.Id = lstCustomers[i].Id;
                    item.ContractCode = lstCustomers[i].ContractCode;
                    item.UserName = lstCustomers[i].UserName;
                    item.Status = lstCustomers[i].Status;
                    item.ModifiedDate = lstCustomers[i].ModifiedDate;
                    dynamic Personal = new ExpandoObject();
                    Personal.Name = lstCustomers[i].Personal.Name;
                    Personal.IdCard = lstCustomers[i].Personal.IdCard;
                    Personal.Phone = lstCustomers[i].Personal.Phone;
                    item.Personal = Personal;
                    dynamic Loan = new ExpandoObject();
                    Loan.Product = lstCustomers[i].Loan != null ? lstCustomers[i].Loan.Product : null;
                    item.Loan = Loan;
                    dynamic Return = new ExpandoObject();
                    Return.Status = lstCustomers[i].Result != null ? lstCustomers[i].Result.Status : null;
                    item.Return = Return;
                    lstCustomerOptimization.Add(item);
                }

                var datasizeInfo = _customerServices.CustomerPagesize(lstCustomers);
                return Ok(new PagingDataResponse
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstCustomerOptimization,
                    pagenumber = pagenumber.HasValue ? (int)pagenumber : 1,
                    totalpage = totalPage,
                    totalrecord = totalrecord
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
        [HttpGet]
        [Route("api/customer/{Id}")]
        public ActionResult<ResponseContext> GetCustomer(string Id)
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
                dynamic result = _customerServices.GetCustomer(Id);
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
        [HttpGet]
        [Route("api/customer/getcustomerbyusername")]
        public ActionResult<ResponseContext> GetCustomerByUserName([FromQuery] string username, [FromQuery] int? pagenumber)
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
                var lstCustomers = new List<Customer>();
                lstCustomers = _customerServices.GetCustomerByUserName(username, pagenumber);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = lstCustomers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/customer")]
        public ActionResult<ResponseContext> Create(Customer customer)
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
                var newCustomer = new Customer();
                newCustomer = _customerServices.CreateCustomer(customer);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = newCustomer
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
        [HttpPost]
        [Route("api/customer/update")]
        public ActionResult<ResponseContext> Update(Customer customer)
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
                long updateCount = _customerServices.UpdateCustomer(customer);
                if (updateCount >= 0)
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.SUCCESS,
                        message = Common.Message.SUCCESS,
                        data = customer
                    });
                }
                else
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = "Lỗi update customer",
                    });
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
        [HttpPost]
        [Route("api/customer/delete")]
        public ActionResult<ResponseContext> Delete([FromBody] string[] IdArray)
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
                long deleteCount = _customerServices.DeleteCustomer(IdArray);
                if (deleteCount >= 0)
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.SUCCESS,
                        message = Common.Message.SUCCESS,
                        data = JsonConvert.SerializeObject("" + deleteCount + " have been deleted")
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = "Cannot delete customer" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
        [HttpGet]
        [Route("api/customer/countstatus")]
        public ActionResult<ResponseContext> CustomerSatusCount([FromQuery] string username, [FromQuery] string greentype)
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
                var statusCount = _customerServices.GetStatusCount(username, greentype);
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = statusCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }

        [HttpPost]
        [Route("api/customer/updateStatus")]
        public ActionResult<ResponseContext> UpdateStatus([FromBody] CustomerUpdateStatusDto dto)
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
                var statusCount = _customerServices.UpdateStatus(dto);
                if (statusCount == 1)
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.SUCCESS,
                        message = Common.Message.SUCCESS,
                        data = statusCount
                    });
                }
                else
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.ERROR,
                        message = Common.Message.CANT_UPDATE_CUSTOMER_ERROR,
                        data = statusCount
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseMessage { status = "ERROR", message = ex.Message });
            }
        }
    }
}