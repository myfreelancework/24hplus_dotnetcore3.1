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

        
        [HttpGet]
        [Route("api/fileuploads/checklist")]
        public ActionResult<ResponseContext> GetChecklistByCustomerId([FromQuery] string customerId)
        {
            try
            {

                if ((bool)HttpContext.Items["isLoggedInOtherDevice"])
                {
                    return Ok(new ResponseContext
                    {
                        code = (int)Common.ResponseCode.IS_LOGGED_IN_ORTHER_DEVICE,
                        message = Common.Message.IS_LOGGED_IN_ORTHER_DEVICE,
                        data = null
                    });
                }
                var data = [
                {
                    "groupId": 22,
                    "groupName": "CMND\/CCCD\/CMQĐ",
                    "mandatory": 1,
                    "hasAlternate": 0,
                    "documents": [
                    {
                        "id": 4,
                        "documentCode": "CivicIdentity",
                        "documentName": "CCCD",
                        "inputDocUid": "559124662598964b868a210069922848",
                        "mapBpmVar": "DOC_CivicIdentity"
                    },
                    {
                        "id": 6,
                        "documentCode": "IdentityCard",
                        "documentName": "CMND",
                        "inputDocUid": "943054199583cdc9c54f601005419993",
                        "mapBpmVar": "DOC_IdentityCard"
                    },
                    {
                        "id": 7,
                        "documentCode": "MilitaryIdentity",
                        "documentName": "CMQĐ",
                        "inputDocUid": "263825031598964c98ee968050949384",
                        "mapBpmVar": "DOC_MilitaryIdentity"
                    }
                    ],
                    "alternateGroups": [
                    
                    ]
                },
                {
                    "groupId": 19,
                    "groupName": "Hộ khẩu",
                    "mandatory": 1,
                    "hasAlternate": 0,
                    "documents": [
                    {
                        "id": 58,
                        "documentCode": "FamilyBook",
                        "documentName": "Sổ hộ khẩu",
                        "inputDocUid": "964050292583cdc9c56ab88092528583",
                        "mapBpmVar": "DOC_FamilyBook"
                    }
                    ],
                    "alternateGroups": [
                    
                    ]
                },
                {
                    "groupId": 26,
                    "groupName": "Hình ảnh khách hàng",
                    "mandatory": 1,
                    "hasAlternate": 0,
                    "documents": [
                    {
                        "id": 23,
                        "documentCode": "FacePhoto",
                        "documentName": "Hình ảnh khách hàng",
                        "inputDocUid": "592712402583cdc9c53fc06001162945",
                        "mapBpmVar": "DOC_FacePhoto"
                    }
                    ],
                    "alternateGroups": [
                    
                    ]
                },
                {
                    "groupId": 23,
                    "groupName": "Sổ tạm trú\/Thẻ tạm trú\/Giấy xác nhận tạm trú",
                    "mandatory": 0,
                    "hasAlternate": 1,
                    "documents": [
                    {
                        "id": 18,
                        "documentCode": "TemporaryResidenceConfirmation",
                        "documentName": "Giấy xác nhận tạm trú",
                        "inputDocUid": "5920212035989652c29e694077488644",
                        "mapBpmVar": "DOC_TemporaryResidenceConfirmation"
                    },
                    {
                        "id": 60,
                        "documentCode": "TemporaryResidenceBook",
                        "documentName": "Sổ tạm trú",
                        "inputDocUid": "316802978583cdc9c553485084790833",
                        "mapBpmVar": "DOC_TemporaryResidenceBook"
                    },
                    {
                        "id": 63,
                        "documentCode": "TemporaryResidenceCard",
                        "documentName": "Thẻ tạm trú",
                        "inputDocUid": "7592442765989653b86a2e9012732831",
                        "mapBpmVar": "DOC_TemporaryResidenceCard"
                    }
                    ],
                    "alternateGroups": [
                    25,
                    24
                    ]
                },
                {
                    "groupId": 30,
                    "groupName": "Sao kê tài khoản thanh toán",
                    "mandatory": 1,
                    "hasAlternate": 0,
                    "documents": [
                    {
                        "id": 57,
                        "documentCode": "StatementPaymentAccount",
                        "documentName": "Sao kê tài khoản thanh toán",
                        "inputDocUid": "8924405085989640f000aa9055057240",
                        "mapBpmVar": "DOC_StatementPaymentAccount"
                    }
                    ],
                    "alternateGroups": [
                    
                    ]
                },
                {
                    "groupId": 34,
                    "groupName": "Phiếu thông tin Khách hàng",
                    "mandatory": 1,
                    "hasAlternate": 0,
                    "documents": [
                    {
                        "id": 49,
                        "documentCode": "CustomerInformationSheet",
                        "documentName": "Phiếu thông tin khách hàng",
                        "inputDocUid": "6970134635989643323f1d9054171098",
                        "mapBpmVar": "DOC_CustomerInformationSheet"
                    }
                    ],
                    "alternateGroups": [
                    
                    ]
                },
                {
                    "groupId": 37,
                    "groupName": "Hồ sơ khác",
                    "mandatory": 0,
                    "hasAlternate": 0,
                    "documents": [
                    {
                        "id": 13,
                        "documentCode": "BirthCertificate",
                        "documentName": "Giấy khai sinh",
                        "inputDocUid": "425720214583cdc9c509104046115087",
                        "mapBpmVar": "DOC_BirthCertificate"
                    },
                    {
                        "id": 34,
                        "documentCode": "Passport",
                        "documentName": "Hộ chiếu",
                        "inputDocUid": "738875901598968883770b8015189815",
                        "mapBpmVar": "DOC_Passport"
                    },
                    {
                        "id": 43,
                        "documentCode": "Other",
                        "documentName": "Khác",
                        "inputDocUid": "508837634598971c46bc117081002262",
                        "mapBpmVar": "DOC_Other"
                    },
                    {
                        "id": 70,
                        "documentCode": "MarriageCertificate",
                        "documentName": "Đăng kí kết hôn",
                        "inputDocUid": "967171635598968971dcdd0064707754",
                        "mapBpmVar": "DOC_MarriageCertificate"
                    }
                    ],
                    "alternateGroups": [
                    
                    ]
                },
                {
                    "groupId": 24,
                    "groupName": "Hóa đơn truyền hình cáp\/điện\/nước\/internet\/DĐTS\/ĐTCĐ",
                    "mandatory": 0,
                    "hasAlternate": 0,
                    "documents": [
                    {
                        "id": 25,
                        "documentCode": "InternetBill",
                        "documentName": "Hóa đơn internet",
                        "inputDocUid": "904371149598964fc6ba0c9093658095",
                        "mapBpmVar": "DOC_InternetBill"
                    },
                    {
                        "id": 28,
                        "documentCode": "WaterBill",
                        "documentName": "Hóa đơn nước",
                        "inputDocUid": "267463210598964e31128a5030554914",
                        "mapBpmVar": "DOC_WaterBill"
                    },
                    {
                        "id": 29,
                        "documentCode": "CableTelevisionBill",
                        "documentName": "Hóa đơn truyền hình cáp",
                        "inputDocUid": "913116455583cdc9c530207061607496",
                        "mapBpmVar": "DOC_CableTelevisionBill"
                    },
                    {
                        "id": 30,
                        "documentCode": "ElectricBill",
                        "documentName": "Hóa đơn điện",
                        "inputDocUid": "4601573265989641f868568061200422",
                        "mapBpmVar": "DOC_ElectricBill"
                    },
                    {
                        "id": 31,
                        "documentCode": "HomePhoneBill",
                        "documentName": "Hóa đơn điện thoại cố định",
                        "inputDocUid": "3506902045989651c778fb0057795362",
                        "mapBpmVar": "DOC_HomePhoneBill"
                    },
                    {
                        "id": 32,
                        "documentCode": "MobilePhoneBill",
                        "documentName": "Hóa đơn điện thoại di động trả sau",
                        "inputDocUid": "9941625265989650ec321a2049101193",
                        "mapBpmVar": "DOC_MobilePhoneBill"
                    }
                    ],
                    "alternateGroups": [
                    
                    ]
                },
                {
                    "groupId": 25,
                    "groupName": "Giấy CN QSH nhà\/ HĐ thế chấp\/HĐ mua bán công chứng",
                    "mandatory": 0,
                    "hasAlternate": 0,
                    "documents": [
                    {
                        "id": 12,
                        "documentCode": "HomeOwnershipCertification",
                        "documentName": "Giấy CN QSH nhà",
                        "inputDocUid": "80233580159896579000269039042203",
                        "mapBpmVar": "DOC_HomeOwnershipCertification"
                    },
                    {
                        "id": 20,
                        "documentCode": "NotarizedHomeSalesContract",
                        "documentName": "HĐ mua bán nhà công chứng",
                        "inputDocUid": "183668936598965a22e3031079994157",
                        "mapBpmVar": "DOC_NotarizedHomeSalesContract"
                    },
                    {
                        "id": 21,
                        "documentCode": "HomeMortgageContract",
                        "documentName": "HĐ thế chấp nhà",
                        "inputDocUid": "9446581325989658bbfea29095214525",
                        "mapBpmVar": "DOC_HomeMortgageContract"
                    }
                    ],
                    "alternateGroups": [
                    
                    ]
                }
                ];
                return Ok(new ResponseContext
                {
                    code = (int)Common.ResponseCode.SUCCESS,
                    message = Common.Message.SUCCESS,
                    data = data
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
    }
}