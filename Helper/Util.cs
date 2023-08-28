using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions; 
using System.Data;

namespace MOFAAPI.Helpers
{
    public static class Util
    {

        // check if the email is valid
        private static bool ValidEmail(this string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static Dictionary<string, string> GetReplacedDictionary(this DataRow dRow)
        {
            Dictionary<string, string> dicReplace = new Dictionary<string, string>();
            foreach (DataColumn dColumn in dRow.Table.Columns)
            {
                dicReplace.Add(dColumn.ColumnName, dRow[dColumn.ColumnName].ToString());
            }
            return dicReplace;
        }


        public static string GetUniqueKeyErrorMessage(string errorMsg)
        {
            string result = string.Empty;

            try
            {
                string constraintCSV = string.Empty;
                string contraintFilePath = string.Empty;

                //if (ConfigurationManager.AppSettings["ContraintFilePath"] != null)
                //    contraintFilePath = Convert.ToString(ConfigurationManager.AppSettings["ContraintFilePath"]);

                if (File.Exists(contraintFilePath))
                {
                    constraintCSV = File.ReadAllText(contraintFilePath);

                    foreach (string constraint in constraintCSV.Split(','))
                    {
                        if (errorMsg.Contains(constraint))
                        {
                            int inderscoreIndex = constraint.LastIndexOf('_');
                            if (inderscoreIndex >= 0 && constraint.Split('_').Length > 2)
                            {
                                result = constraint.Substring(inderscoreIndex + 1, (constraint.Length - (inderscoreIndex + 1)));

                                if (result.ToLower() == "code")
                                    result = "msgCodeAlreadyExists";
                                else if (result.ToLower() == "name")
                                    result = "msgNameAlreadyExists";
                                else if (result.ToLower() == "id")
                                    result = "msgIDAlready";
                                else if (result.ToLower() == "serialnumber")
                                    result = "msgSerialNumberExist";

                                break;
                            }
                            else
                            {
                                result = "msgAlreadyExist";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public static string GetBinaryDataStringFromBase64String(string base64String)
        {
            string result = string.Empty;

            try
            {
                byte[] byteResult = GetBinaryDataFromBase64String(base64String);
                if (byteResult != null)
                    result = System.Text.Encoding.UTF8.GetString(byteResult);
            }
            catch (Exception)
            {
                result = "";
            }

            return result;
        }

       
        public static byte[] GetBinaryDataFromBase64String(string base64String)
        {
            byte[] fileData = null;

            try
            {
                if (!string.IsNullOrEmpty(base64String))
                {
                    fileData = Convert.FromBase64String(base64String);
                }

            }
            catch (Exception)
            {
                fileData = null;
            }

            return fileData;
        }

        
    }
}