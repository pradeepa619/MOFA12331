using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOFAAPI.OracleDataAccess;

using IronPdf;
using IronPdf.Editing;
using IronBarCode;
using MOFAAPI.Models;
using System.Linq.Expressions;
using MOFAAPI.Helpers;

namespace MOFAAPI.Controllers
{
    [Route("api/Entity")]
    [ApiController]
    public class EntityController : ControllerBase
    {
        private IConfiguration _config;
        private IDataAccess daObj;
        private static ILogger<EntityController> _logger;

        public EntityController(ILogger<EntityController> logger, IConfiguration iConfig, IDataAccess _daObj)
        {
            _logger = logger;
            _config = iConfig;
            daObj = _daObj;
        }

        [HttpPost]
        [Route("AddAttestation")]
        public IActionResult AddAttestation([FromBody] dynamic paramsObject)
        {            
            dynamic dynamicResponse = new DynamicData();
            try
            {               
                string json = paramsObject.ToString().Replace("\r\n", "");
                Newtonsoft.Json.Linq.JObject jobj = Newtonsoft.Json.Linq.JObject.Parse(json);
                List<object> paramobj = new List<object>();
                string attestFile = jobj.GetValue("attestFile").ToString();
              
                string sAttestDocName = jobj.GetValue("attestDocName").ToString();
                string sreqAppNumber = jobj.GetValue("reqAppNumber").ToString();
                string sentitycode = jobj.GetValue("entityCode").ToString();
                string sRequestername = jobj.GetValue("requesterName").ToString();
                if (attestFile.Length>0)
                {                                                           
                    foreach (var obj in jobj)
                    {
                        paramobj.Add(obj.Value);
                    }
                    object[] reqAttestation = paramobj.ToArray();
                    Models.Response res = daObj.AddAttestation(reqAttestation);
                    if (res.responsecode >= 1)
                    {
                        string base64 = PDFStamping(sentitycode, attestFile, sreqAppNumber, sAttestDocName);
                        dynamicResponse.requestorCode = sentitycode;
                        dynamicResponse.reqAppNumber = sreqAppNumber;
                        dynamicResponse.requesterName = sRequestername;
                        dynamicResponse.attestNo = "AECI29032022H7CZ";
                        dynamicResponse.docOwnerEmail = jobj.GetValue("emailId");
                        dynamicResponse.attestDocName = sAttestDocName;
                        dynamicResponse.attestedFile = base64;
                        dynamicResponse.statusCode = "10000";
                        return new ObjectResult(new { Description = "Success", ResponseCode = 200, data = dynamicResponse });
                    }
                    else
                    {
                        dynamicResponse.reqAppNumber = sreqAppNumber;
                        dynamicResponse.statusCode = "20001";
                        return new ObjectResult(new { Description = "Failed", ResponseCode = 400, data = dynamicResponse });
                    }
                }
                else
                {
                    return new ObjectResult(new { Description = "Failed", ResponseCode = 400, data = "Filr is empty" });
                }
            }catch(Exception ex)
            {
                dynamicResponse.reqAppNumber = "";
                dynamicResponse.statusCode = "20001";
                dynamicResponse.message = ex.Message.ToString();
                _logger.LogError(ex.Message.ToString());
                return new ObjectResult(new { Description = "Internal Server Error.", ResponseCode = 500, Data = dynamicResponse });
            }
        }        
        public string PDFStamping(string entitycode,string sAttestfile,string sreqApp,string sAttestdocname)
        {
            IronPdf.License.LicenseKey = "IRONSUITE.ITALLWIN.GMAIL.COM.611-B010E70C71-AF4KMFRP663NVPNH-BQ535EUQWPB4-RHF2SK5U7FRD-2PYJLB4B4Y4V-OJJT2MBGH4RS-C5DGLUI7B2LS-Z7NOEC-TFOCP3KBHXWKEA-DEPLOYMENT.TRIAL-UDYPII.TRIAL.EXPIRES.20.AUG.2023";
            int result = 0; string base64;
            try
            {               
                string strQrcode = QRCodeWriter.CreateQrCode(entitycode, 130).ToHtmlTag();
                string directorypath = _config.GetValue<string>("EntityAttestationPath");
                string sAssetPath = _config.GetValue<string>("HTMLAssestsPath");
                directorypath = directorypath + DateTime.Now.Date.ToString("ddmmyyyy") + "\\" + sreqApp + "\\";
                System.IO.Directory.CreateDirectory(directorypath);
                string fi = directorypath + sAttestdocname + "_draft";
                string dPath = fi;
                byte[] decodedByteArray = Convert.FromBase64String(sAttestfile);
                System.IO.File.WriteAllBytes(dPath, decodedByteArray);
                PdfDocument pdf = PdfDocument.FromFile(dPath);
                string strH = "<table width='100%; style='padding-bottom:-20px'><tr><td align='right'>" + strQrcode + "<img src='assets/UAEStamp.png' style='width: 150px;'></td></tr></table>";
                var backgroundStamp = new HtmlStamper(strQrcode)
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Html = strH,
                    VerticalOffset = new Length(-1, MeasurementUnit.Centimeter),
                    HorizontalOffset = new Length(-1, MeasurementUnit.Centimeter),
                    IsStampBehindContent = true,
                    HtmlBaseUrl = new Uri(sAssetPath)
                };
                pdf.ApplyStamp(backgroundStamp);
                pdf.SaveAs(directorypath + sAttestdocname + "_stamped.pdf");
                string fpath = directorypath + sAttestdocname + "_stamped.pdf";
                byte[] byteArray = System.IO.File.ReadAllBytes(fpath);
                 base64 = Convert.ToBase64String(byteArray, 0, byteArray.Length);                
            }
            catch(Exception e) { throw e; }
            return base64;
            }        
    }
}
