using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOFAAPI.Helpers;
using MOFAAPI.Models;
using MOFAAPI.OracleDataAccess;

namespace MOFAAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class LoginController : ControllerBase
    {
        //private DataAccess.DataAccess daObj = new DataAccess.DataAccess();
        private static ILogger<LoginController> _logger;
        private IDataAccess daObj;
        private RSAHelper rsa = new RSAHelper();
        public LoginController(ILogger<LoginController> logger,IDataAccess _daObj)
        {
            _logger = logger;
            daObj = _daObj;
        }

        // [Authorize]
        [Route("getUserDetails")]
        [HttpPost]
        [EnableCors("AllowOrigin")]
        public IActionResult getUserDetails([FromBody] dynamic UserInfo)
        {            
            try
            {
                if (UserInfo.ToString() != "")
                {                    
                    string json = UserInfo.ToString().Replace("\r\n", "");
                    Newtonsoft.Json.Linq.JObject jobj = Newtonsoft.Json.Linq.JObject.Parse(json);
                    List<object> paramobj = new List<object>();
                    foreach (var obj in jobj)
                    {
                        if (obj.Key == "Password")
                        {
                            object sDecryptedPwd = rsa.Decrypt(obj.Value.ToString());
                            paramobj.Add(sDecryptedPwd);
                        }else
                        paramobj.Add(obj.Value);
                    }
                    object[] reqUserInfo = paramobj.ToArray();
                    UserInfo userDet = daObj.getUserDetails(reqUserInfo);
                    if(userDet.username!=null)
                    {
                        return new ObjectResult(new { Description = "Success", ResponseCode = 200, Data = userDet });
                    }
                    else
                        return new ObjectResult(new { Description = "Failed", ResponseCode = 500 });
                }
                else
                { return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = "Value is null" }); }
            }
            catch (Exception ex)
            {               
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }
        //[Authorize]
        /*[Route("PostUserIdea")]
        [HttpPost]
        public IActionResult PostUserIdea([FromBody] AfkariModels.postIdea paramsObject)        
        {            
            try
            {
                AfkariModels.response resp = daObj.postUserIdea(paramsObject);                
                return new ObjectResult(new { Description = resp.respStatus, ResponseCode = 200, Data = resp });                
            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                _logger.LogError(ex.Message.ToString());
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }

        [Route("AddCampaign")]
        [HttpPost]
        public IActionResult AddCampaign([FromBody] dynamic paramsObject)       
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
                AfkariModels.response resp = daObj.AddCampaign(reqCampaign);
                //List<Models.Ideas> lstIdeas = daObj.postUserIdea(paramsObject);
                if (resp.message == "Success")
                {
                    return new ObjectResult(new { Description = "Success", ResponseCode = 200, Data = resp });
                }
                else
                {
                    return new ObjectResult(new { Description = "Failed", ResponseCode = 200, Data = resp });
                }
            }
            catch (Exception ex)
            {             
                _logger.LogError(ex.Message.ToString());
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }
        [Route("getCampaign")]
        [HttpPost]
        public IActionResult getCampaign([FromBody] afkariUser UserInfo)
        {
            dynamic DynamicDataList = new DynamicData();
            try
            {                
                if (UserInfo != null)
                {
                    List<AfkariModels.campaign> lstCampaign = daObj.getCampaign(UserInfo.UserId);
                    return new ObjectResult(new { Description = "Success", ResponseCode = 200, Data = lstCampaign });
                }
                else
                {
                    return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = "Value is null" });
                }               
            }
            catch (Exception ex)
            {               
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }*/
    }
}
