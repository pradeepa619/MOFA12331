using MOFAAPI.Helpers;
using MOFAAPI.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using WebApi.Controllers;
//using IdeationProjAPI.Authorization;
using MOFAAPI.DataAccess;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Dynamic;
using static MOFAAPI.Models.AfkariModels;
using System.Text.Json;

namespace MOFAAPI.Controllers
{   
    //[EnableCors("CorsApi")]
    [Route("api/AFKari")]
    [ApiController]
    public class AfkariController : ControllerBase
    {
        private DataAccess daObj = new DataAccess();
        private static ILogger<AfkariController> _logger;
        public AfkariController(ILogger<AfkariController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string get()
        {
            return "hello";
        }

        // [Authorize]
        [Route("getUserIdeas")]
        [HttpPost]
        public IActionResult getUserIdeas([FromBody] afkariUser UserInfo)
        {
            //dynamic DynamicDataList = new DynamicData();
            try
            {
                if(UserInfo.ToString()!="")
                { 
                    //var converter = new ExpandoObjectConverter();
                    //var exObj = JsonConvert.DeserializeObject<ExpandoObject>(jsondata.ToString(), converter);
                    //string sUserID = exObj.sUserID.ToString();
                    //afkariUser user = JsonConvert.DeserializeObject<afkariUser>(jsondata.ToString());
                    List<AfkariModels.Ideas> lstIdeas = daObj.getUserIdeas(UserInfo.UserId);
                    return new ObjectResult(new { Description = "Success", ResponseCode = 200, Data = lstIdeas });
                }
                else
                { return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = "Value is null" }); }
            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                //_logger.LogError(ex.Message.ToString());
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }

        //[Authorize]
        [Route("PostUserIdea")]
        [HttpPost]
        public IActionResult PostUserIdea([FromBody] AfkariModels.postIdea paramsObject)
        //public IActionResult PostUserIdea(string sIdeaOwner)
        {
            //dynamic dynamicResponse = new DynamicData();
            try
            {
                //dynamic dynResponse = sIdeaOwner;
                //dynamicResponse =
                //AfkariModels.response resp = daObj.postUserIdea(sIdeaOwner);
                //Assing object array
                
                AfkariModels.response resp = daObj.postUserIdea(paramsObject);
                //if (resp.message=="Success")
                //{
                    return new ObjectResult(new { Description = resp.respStatus, ResponseCode = 200, Data = resp });
                //}
                /*else
                {
                    return new ObjectResult(new { Description = "Failed", ResponseCode = 200, Data = resp });
                }*/
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
        //public IActionResult AddCampaign(string sIdeaOwner)
        {
            //dynamic dynamicResponse = new DynamicData();
            try
            {
                //dynamic dynResponse = sIdeaOwner;
                //dynamicResponse =
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
                //Get the User defined Error message
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
                //if (paramsObject != null)
                // {
                if (UserInfo != null)
                {
                    List<AfkariModels.campaign> lstCampaign = daObj.getCampaign(UserInfo.UserId);
                    return new ObjectResult(new { Description = "Success", ResponseCode = 200, Data = lstCampaign });
                }
                else
                {
                    return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = "Value is null" });
                }                
                //}
                //else
                //{ return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = "Value is null" }); }
            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                //_logger.LogError(ex.Message.ToString());
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = ex.Message });
            }
        }
    }
}
