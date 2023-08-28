using Newtonsoft.Json.Linq;
using System.Net.Mail;

namespace MOFAAPI.Models
{    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public decimal useruno { get; set; }
        public string emailaddress { get; set; }
    }
    public class companydetails
    {
        public decimal useruno { get; set; }
        public string companyname { get; set; }
        public decimal tradelicensenumber { get; set; }
        public string tradelicenseissuedate { get; set; }
        public string tradelicenseexpirydate { get; set; }
        public decimal licenseissuingauthority { get; set; }
        public string companyregisteredemailaddress { get; set; }
        public decimal companycontactnumber { get; set; }
        public int HASATTACHMENT { get; set; }
        public string attachment { get; set; }
        public string representativeemailaddress { get; set; }
        public string mobilenumber { get; set; }
        
    }

    public class registerCompany
    {
        public string emiratesid { get; set; }
        public string uuid { get; set; }
        public string token { get; set; }
        public decimal useruno { get; set; }
        public string companyname { get; set; }
        public decimal tradelicensenumber { get; set; }
        public string tradelicenseissuedate { get; set; }
        public string tradelicenseexpirydate { get; set; }
        public decimal licenseissuingauthorityuno { get; set; }
        public string companyregisteredemailaddress { get; set; }
        public decimal companycontactnumber { get; set; }
        public decimal hasattachment { get; set; }
        public string attachment { get; set; }
        public string representativeemailaddress { get; set; }
        public string mobilenumber { get; set; }
    }

    public class companyattachment
    {

    }
    public class Response
    {
        public int responsecode { get; set; }
        public string message { get; set; }
    }

    public class attachment
    {
        public byte attachmentfile { get; set; }
    }


}
