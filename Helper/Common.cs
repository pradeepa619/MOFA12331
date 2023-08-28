using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using MOFAAPI.Helpers;
using NLog;

namespace MOFAAPI.Helpers
{
    public class Common
    {
        private NLog.ILogger g_logger;

        private byte[] _keyByte = { };
        private char[] ldapFilterEscapeSequence = new char[] { '\\', '*', '(', ')', '\0', '/' };
        private string[] ldapFilterEscapeSequenceCharacter = new string[] { "\\5c", "\\2a", "\\28", "\\29", "\\00", "\\2f" };
        private char[] ldapDnEscapeSequence = new char[] { '\\', ',', '+', '"', '<', '>', ';' };
        public string GetErrorLogPath()
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

            var connectionStringConfig = builder.Build();
            if (connectionStringConfig != null)
                return connectionStringConfig.GetConnectionString("ErrorLog");
            else
                return string.Empty;

        }

        public Common()
        {

            g_logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        /*public AppSettings GetEmailHostSetting()
        {
            AppSettings objAppSettings = new AppSettings();
            try
            {
                var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

                var connectionStringConfig = builder.Build();
                if (connectionStringConfig != null)
                {
                    objAppSettings.EmailFromAddress = connectionStringConfig.GetSection("AppSettings").GetSection("EmailFromAddress").Value;
                    objAppSettings.EmailHost = connectionStringConfig.GetSection("AppSettings").GetSection("EmailHost").Value;
                    objAppSettings.EmailUserName = connectionStringConfig.GetSection("AppSettings").GetSection("EmailUserName").Value;
                    objAppSettings.EmailKey = connectionStringConfig.GetSection("AppSettings").GetSection("EmailKey").Value;
                    objAppSettings.EmailPort = connectionStringConfig.GetSection("AppSettings").GetSection("EmailPort").Value;
                    objAppSettings.EmailSSL = Convert.ToBoolean(connectionStringConfig.GetSection("AppSettings").GetSection("EmailSSLEnabled").Value); 
                    objAppSettings.AppUrl = connectionStringConfig.GetSection("AppSettings").GetSection("AppUrl").Value;
                }

            }
            catch (Exception exp)
            {
                g_logger.LogException(NLog.LogLevel.Error, exp.Message.ToString(), exp);

            }

            return objAppSettings;
        }*/


        public static string SanitizeString(string text)
        {
            List<int> openTagIndexes = Regex.Matches(text, "<").Cast<Match>().Select(m => m.Index).ToList();
            List<int> closeTagIndexes = Regex.Matches(text, ">").Cast<Match>().Select(m => m.Index).ToList();
            if (closeTagIndexes.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                int previousIndex = 0;
                foreach (int closeTagIndex in closeTagIndexes)
                {
                    var openTagsSubset = openTagIndexes.Where(x => x >= previousIndex && x < closeTagIndex);
                    if (openTagsSubset.Count() > 0 && closeTagIndex - openTagsSubset.Max() > 1)
                    {
                        sb.Append(text.Substring(previousIndex, openTagsSubset.Max() - previousIndex));
                    }
                    else
                    {
                        sb.Append(text.Substring(previousIndex, closeTagIndex - previousIndex + 1));
                    }
                    previousIndex = closeTagIndex + 1;
                }
                if (closeTagIndexes.Max() < text.Length)
                {
                    sb.Append(text.Substring(closeTagIndexes.Max() + 1));
                }
                return sb.ToString();
            }
            else
            {
                return text;
            }
        }

        public void LogError(Exception exp)
        {
            g_logger.LogException(NLog.LogLevel.Error, exp.Message.ToString(), exp);

        }

        public static string getServicebasepath()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var appsettings = builder.Build();
                if (appsettings != null)
                    return Convert.ToString(appsettings.GetSection("AppSettings").GetSection("ServicebaseUrl").Value);
                else
                    return string.Empty;
            }
            catch (Exception e) { throw e; }
        }

        public string getADPath()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var appsettings = builder.Build();
                if (appsettings != null)
                    return Convert.ToString(appsettings.GetSection("AppSettings").GetSection("ADUrl").Value);
                else
                    return string.Empty;
            }
            catch (Exception e) { throw e; }
        }

        public string getADDomain()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var appsettings = builder.Build();
                if (appsettings != null)
                    return Convert.ToString(appsettings.GetSection("AppSettings").GetSection("ADDomain").Value);
                else
                    return string.Empty;
            }
            catch (Exception e) { throw e; }
        }


        /// <summary>
        /// Escape a string for usage in an LDAP DN to prevent LDAP injection attacks.
        /// There are certain characters that are considered special characters in a DN.
        /// The exhaustive list is the following: ',','\','#','+','<','>',';','"','=', and leading or trailing spaces
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string EscapeForDN(string name)
        {
            StringBuilder sb = new StringBuilder();
 

            for (int i = 0; i < name.Length; i++)
            {
                char curChar = name[i];
                switch (curChar)
                {
                    case ',':
                        sb.Append(@"\,");
                        break;
                    case '+':
                        sb.Append(@"\+");
                        break;
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '<':
                        sb.Append(@"\<");
                        break;
                    case '>':
                        sb.Append(@"\>");
                        break;
                    case ';':
                        sb.Append(@"\;");
                        break;
                    default:
                        sb.Append(curChar);
                        break;
                }
            }
 

            return sb.ToString();
        }



        public string CanonicalizeStringForLdapFilter(string userInput)
        {
            if (String.IsNullOrEmpty(userInput))
            {
                return userInput;
            }

            string name = (string)userInput.Clone();

            for (int charIndex = 0; charIndex < ldapFilterEscapeSequence.Length; ++charIndex)
            {
                int index = name.IndexOf(ldapFilterEscapeSequence[charIndex]);
                if (index != -1)
                {
                    name = name.Replace(new String(ldapFilterEscapeSequence[charIndex], 1), ldapFilterEscapeSequenceCharacter[charIndex]);
                }
            }

            return name;
        }



        public string CanonicalizeStringForLdapDN(string userInput)
        {
            if (String.IsNullOrEmpty(userInput))
            {
                return userInput;
            }

            string name = (string)userInput.Clone();

            for (int charIndex = 0; charIndex < ldapDnEscapeSequence.Length; ++charIndex)
            {
                int index = name.IndexOf(ldapDnEscapeSequence[charIndex]);
                if (index != -1)
                {
                    //name = name.Replace(new string(ldapDnEscapeSequence[charIndex], 1), @"\" + ldapDnEscapeSequence[charIndex]);
                    name = name.Replace(ldapDnEscapeSequence[charIndex], ldapDnEscapeSequence[charIndex]);
                }
            }

            return name;
        }

        public bool IsUserGivenStringPluggableIntoLdapSearchFilter(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return true;
            }

            if (userInput.IndexOfAny(ldapDnEscapeSequence) != -1)
            {
                return false;
            }

            return true;
        }

        public bool IsUserGivenStringPluggableIntoLdapDN(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return true;
            }

            if (userInput.IndexOfAny(ldapFilterEscapeSequence) != -1)
            {
                return false;
            }

            return true;
        }

        public static string GetSecratValue()
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

            var connectionStringConfig = builder.Build();
            if (connectionStringConfig != null)
                return connectionStringConfig.GetSection("AppSettings").GetSection("Secret").Value;
            else
                return "";  
        }

        public string GetJWTSecurityTokenForSmartOffice(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(GetSecratValue());
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        /// <summary>
        /// Escape a string for usage in an LDAP DN to prevent LDAP injection attacks.
        /// </summary>
        public string EscapeForSearchFilter(string filter)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < filter.Length; i++)
            {
                char curChar = filter[i];
                switch (curChar)
                {
                    case '\\':
                        sb.Append("\\5c");
                        break;
                    case '*':
                        sb.Append("\\2a");
                        break;
                    case '(':
                        sb.Append("\\28");
                        break;
                    case ')':
                        sb.Append("\\29");
                        break;
                    case '\u0000':
                        sb.Append("\\00");
                        break;
                    default:
                        sb.Append(curChar);
                        break;
                }
            }
            return sb.ToString();
        }
        public string GetConnectionStringName()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            var connectionStringConfig = builder.Build();
            if (connectionStringConfig != null)
            {
                StringBuilder connectionSB = new
                //StringBuilder(Utilities.EncryptDecrypt.Decryption(connectionStringConfig.GetConnectionString("DbConnection").Replace("~", "\\").Replace("\\\\", "\\")));
                StringBuilder(connectionStringConfig.GetConnectionString("DbConnection").Replace("~", "\\").Replace("\\\\", "\\"));
                //connectionSB.Append("Column Encryption Setting=enabled");
                return connectionSB.ToString();
            }
            else
                return string.Empty;
            }

        public string CheckErrorMessage(string InputErrrText)
        {
            string ErrorMsg = string.Empty;

            if (InputErrrText.ToString().ToUpper().Contains(("Violation of UNIQUE KEY constraint").ToUpper()))
            {
                ErrorMsg = "Error in Transaction; Duplicate Items are identified";
            }
            else if (InputErrrText.ToString().ToUpper().Contains(("401").ToUpper()))
            {
                ErrorMsg = "Error in Transaction; 401 Unauthorized action";
            }
            else if (InputErrrText.ToString().ToUpper().Contains("INDEX WAS OUT OF RANGE."))
            {
                ErrorMsg = "Error in Transaction; Please check the values. Some blank values are identified";
            }
            else if (InputErrrText.ToString().ToUpper().Contains("DELETE DISABLED"))
            {
                ErrorMsg = "Delete disabled. Attribute Mapping exists.";
            }
            return ErrorMsg;
        }


        public string GetdataLogPath()
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json");

            var connectionStringConfig = builder.Build();
            if (connectionStringConfig != null)
                return connectionStringConfig.GetConnectionString("DataLog");
            else
                return string.Empty;

        }

        public SecureString convertToSecureString(string strPassword)
        {
            var secureStr = new SecureString();
            if (strPassword.Length > 0)
            {
                foreach (var c in strPassword.ToCharArray()) secureStr.AppendChar(c);
            }
            return secureStr;
        }

        public string convertToUNSecureString(SecureString secstrPassword)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secstrPassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }


        public static string getSrcFilepath()
        {
            try
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var appsettings = builder.Build();
                if (appsettings != null)
                    return Convert.ToString(appsettings.GetSection("AppSettings").GetSection("SrcFilepath").Value);
                else
                    return string.Empty;
            }
            catch (Exception e) { throw e; }
        }

      
        //public bool ValidateUserkey(string expectedKey, int expectedValue)
        //{
        //    int actualValue;
        //    return allowedApps.TryGetValue(expectedKey, out actualValue) &&
        //           actualValue == expectedValue;
        //}

        public static string GetFileContentType(string ext)
        {

            string contentType = "";
            switch (ext)
            {
                case ".txt":
                    contentType = "text/plain";
                    break;
                case ".doc":
                case ".dot":
                    contentType = "application/msword";
                    break;
                case ".docx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".dotx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                    break;
                case ".xls":
                case ".xlt":
                case ".xla":
                    contentType = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".xltx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                    break;
                case ".ppt":
                case ".pot":
                case ".pps":
                case ".ppa":
                    contentType = "application/vnd.ms-powerpoint";
                    break;
                case ".pptx":
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case ".potx":
                    contentType = "application/vnd.openxmlformats-officedocument.presentationml.template";
                    break;
                case ".mdb":
                    contentType = "application/vnd.ms-access";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".avi":
                    contentType = "video/x-msvideo";
                    break;
                case ".mp4":
                    contentType = "video/mpeg";
                    break;
                case ".mpeg":
                    contentType = "video/mpeg";
                    break;
                case ".mpg":
                    contentType = "video/mpeg";
                    break;
                case ".zip":
                    contentType = "application/zip"; //application/octet-stream
                    break;
            }
            return contentType;
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public string GetFunctionName()
        {
            string functionName = string.Empty;
            try
            {
                // get call stack
                StackTrace stackTrace = new StackTrace();

                // get calling method name
                functionName = stackTrace.GetFrame(2).GetMethod().Name + " - Line Number : " + stackTrace.GetFrame(2).GetFileLineNumber();
            }
            catch (Exception exp)
            {
                throw exp;
            }

            return functionName;
        }

        public string GetUserException(string exception)
        {
            string errorMessage = string.Empty;
            try
            {

                if (!string.IsNullOrEmpty(exception))
                {
                    //Log the Exception
                    //g_Logger.LogException(GetFunctionName(), exception);
                    errorMessage = "Sorry unable to proceed";
                }

                if (exception.ToUpper().Trim().Contains("IX_TB"))
                {
                    errorMessage = "Already Exists";
                }
                else if (exception.ToUpper().Trim().Contains("UNIQUE KEY"))
                {
                    errorMessage = "Cannot add duplicate item / Item already exists";
                }

                else
                {
                    string uniqueKeyErrorMessage = Util.GetUniqueKeyErrorMessage(exception);

                    if (!string.IsNullOrEmpty(uniqueKeyErrorMessage))
                        errorMessage = uniqueKeyErrorMessage;
                    else
                        errorMessage = "msgErrorOccured";
                }

            }

            catch (Exception ex) 
            {
                throw ex;
            }

            return errorMessage;
        }

        /// <summary>
        /// Set Properties for an object dynamically
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public static void SetProperty(object obj, string propName, object value)
        {
            Type typ = obj.GetType();
            PropertyInfo info = typ.GetProperty(propName);

            if (info == null)
                return;
            if (!info.CanWrite)
                return;
            switch (Convert.ToString(info.PropertyType.FullName))
            {
                case "System.String":
                    info.SetValue(obj, value, null);
                    break;

                case "System.DateTime":
                    info.SetValue(obj, GConvert.ToDateTime(value), null);
                    break;

                case "System.Decimal":
                    info.SetValue(obj, GConvert.ToDecimal(value), null);
                    break;

                default:
                    info.SetValue(obj, value, null);
                    break;
            }
        } 
        #region Enumerators

        public enum DocumentType
        {
            Maintenance = 1,
            Schedule = 2

        }

        public enum DocumentDetails
        {
            Registration = 1, 
            Visa = 5,
            Passport = 6
        }

        public enum UserType
        {
            SuperAdmin = 1,
            Provider = 2,
            Admin = 3,
            User = 4,
            Authority = 5
        }

        public enum UserMasterStatus
        {
            ApprovalPending = 1,
            Approved = 2,
            Rejected = 3
        }

        public enum NonUIMasters
        {
            Division = 1,
            Department = 2, 
        }

        public enum DateTimeSecondsResetFor
        {
            FromDate = 1,
            ToDate = 2,
            Common = 3,
        }
        public enum EmailFor
        {
            NewUserCreation = 1,
            ResetUserPassword = 2, 
            RecoveryEmailVerification = 3
        }
         
        public enum FillUser
        {
            UserInfo = 0,
            UserWithDivDept = 1,
            UserWithEmail = 2 
        }

        

        public enum LoginErrorCode
        {
            LoginSuccess = 1,
            LoginFailed = 2,
            UserBlocked = 3,
            UserNotExists = 4,
            DBConnectionFailed = 5,
            DBConnectionLoginFailed = 6,
            OperationUnSuccessFul = 7

        }
         
        #endregion Enumerators
    }

    public class WSResult
    {
        public int IntResult { get; set; }

        public Int64 Int64Result { get; set; }

        public Int64 OrderUno { get; set; }

        public string StringResult { get; set; }

        public bool BoolResult { get; set; }
    }

    public class EmailData
    {
        public string EmailId { get; set; }

        public string CCEmailId { get; set; }

        public string DisplayName { get; set; }

        public string RefData { get; set; }
    }
}