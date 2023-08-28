using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOFAAPI.Helpers;
using System.Net.NetworkInformation;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using MOFAAPI.OracleDataAccess;

namespace MOFAAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private IConfiguration _config;
        private IDataAccess daObj;
        private static ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger, IConfiguration iConfig, IDataAccess _daObj)
        {
            _logger = logger;
            _config = iConfig;
            daObj = _daObj;
        }

        [Route("AddCompany")]
        [HttpPost]
        public IActionResult AddCompany([FromBody] dynamic paramsObject)        
        {            
            try
            {
                string json = paramsObject.ToString().Replace("\r\n", "");
                Newtonsoft.Json.Linq.JObject jobj = Newtonsoft.Json.Linq.JObject.Parse(json);
                List<object> paramobj = new List<object>();
                foreach (var obj in jobj)
                {
                    paramobj.Add(obj.Value);
                }
                object[] reqCampaign = paramobj.ToArray();
                Models.Response res = daObj.addCompany(reqCampaign);
                if (res.responsecode==1)
                {
                    return new ObjectResult(new { Description = "Success", ResponseCode = 200,data=res });
                }
                else
                {
                    return new ObjectResult(new { Description = "Failed", ResponseCode = 200});
                }
            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                _logger.LogError(ex.Message.ToString());
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }
        [Route("RegisterCompany")]
        [HttpPost]       
        public IActionResult RegisterCompany()
        {
            try
            {
                int iFilecount = Request.Form.Files.Count;
                var fileInfo = new FileInfo("D:\\");//default value
                object paramsObject = Request.Form["Data"];
                string json = paramsObject.ToString().Replace("\r\n", "");
                Newtonsoft.Json.Linq.JObject jobj = Newtonsoft.Json.Linq.JObject.Parse(json);
                List<object> paramobj = new List<object>();
                string sCompanyname = jobj.GetValue("companyname").ToString();
                if (iFilecount > 0) { fileInfo = AttachFiles(Request, sCompanyname); }                
               
                foreach (var obj in jobj)
                {
                    if (iFilecount > 0 && obj.Key == "attachment")
                    {
                        paramobj.Add(fileInfo.FullName);
                    }
                    else
                    {
                        paramobj.Add(obj.Value);
                    }
                }
                object[] reqRegisterCompany = paramobj.ToArray();
                Models.Response res = daObj.registerCompany(reqRegisterCompany);
                if (res.responsecode >= 1)//new request -- for new request requestno will be responsecode
                {
                    //replace filename with requestno
                    try
                    {
                        if (iFilecount > 0)
                        {
                            string sRequestno = res.message;                            
                            Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(fileInfo.FullName, res.responsecode + fileInfo.Name.Replace("_draft",""));
                        }
                    }
                    catch(Exception e) { _logger.LogError(e.Message.ToString()); };
                    return new ObjectResult(new { Description = "Success", ResponseCode = 200,data=res });
                }
                else
                {
                    return new ObjectResult(new { Description = "Failed", ResponseCode = 400,data=res });
                }
            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                _logger.LogError(ex.Message.ToString());
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }
        public FileInfo AttachFiles(HttpRequest req,string sCompanyName)
        {
                var filePath = new FileInfo("D:\\");            
                using (var stream = Request.Form.Files[0].OpenReadStream())
                {
                    string fileName = Path.GetFileName(Request.Form.Files[0].FileName);
                    string ext = Path.GetExtension(fileName);
                    string contentType = Common.GetFileContentType(ext);
                    if (contentType != String.Empty)
                    {
                        //int userUno = (Request.Form != null) ? GConvert.ToInt16(Request.Form["UserUno"][0]) : 0;
                        string directorypath = _config.GetValue<string>("AttachmentPath");                    
                        directorypath = directorypath + DateTime.Now.Date.ToString("ddmmyyyy") + "\\" + sCompanyName + "\\";
                        System.IO.Directory.CreateDirectory(directorypath);
                        string fi = directorypath + fileName + "_draft";
                        filePath = new FileInfo(fi);
                        Stream streamToWriteTo = System.IO.File.Open(fi, FileMode.OpenOrCreate);
                        stream.CopyTo(streamToWriteTo);
                        streamToWriteTo.Close();
                    }
                 }
                return filePath;
        }
        
        [HttpPost]
        [Route("UploadInitFiles")]
        public IActionResult UploadInitFiles()
        {
            dynamic dynamicResponse = new DynamicData();
            try
            {
                    dynamic dynResponse = new DynamicData();var filePath=new FileInfo("D:\\");
                    using (var stream = Request.Form.Files[0].OpenReadStream())
                    {                        
                            string fileName = Path.GetFileName(Request.Form.Files[0].FileName);
                            string ext = Path.GetExtension(fileName);
                            string contentType = Common.GetFileContentType(ext);                            
                            if (contentType != String.Empty)
                            {
                                int userUno = (Request.Form != null) ? GConvert.ToInt16(Request.Form["UserUno"][0]) : 0;
                                string directorypath = _config.GetValue<string>("AttachmentPath");
                                directorypath = directorypath + DateTime.Now.Date.ToString("ddmmyyyy") + "\\" + userUno+"\\";
                                System.IO.Directory.CreateDirectory(directorypath);
                                string fi = directorypath + fileName;
                                filePath = new FileInfo(fi);                                
                                Stream streamToWriteTo = System.IO.File.Open(fi, FileMode.OpenOrCreate);                            
                                stream.CopyTo(streamToWriteTo);                                                                    
                                streamToWriteTo.Close();
                            }
                 }
                    dynamicResponse.filepath = filePath.FullName;                    
                    Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(filePath.FullName, "1_" + filePath.Name);
                    return new ObjectResult(new { Description = "Success", ResponseCode = 200, data = dynamicResponse });
            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                _logger.LogError(ex.Message.ToString());
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }
    }
}