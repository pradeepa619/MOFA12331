using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using MOFAAPI.Helpers;
using MOFAAPI.Models;
using Newtonsoft.Json.Linq;
using System.Transactions;
using System.Data;
using System.Dynamic;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Reflection;
using static MOFAAPI.Helpers.OracleHelper;
using Microsoft.AspNetCore.CookiePolicy;
using NLog;
using MOFAAPI.OracleDataAccess;

namespace MOFAAPI.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private Nullable<Int64> g_OutParameter64 = 0;
        private string g_OutErrorMessage = string.Empty;

        private static String _connectionString;
        public IConfiguration Configuration { get; }
        private Common g_Common = new Common();
        private NLog.ILogger g_logger;        
        public DataAccess()
        {
            if (_connectionString == null)
            {
                _connectionString = g_Common.GetConnectionStringName();
            }
            NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
        }

        #region UserValidationCode
        /*public DataSet GetUserValidation(string Token1, string Token2, int NTLoginValid)
        {
            DataSet dsResult = new DataSet();
            try
            {
                dsResult = (DataSet)OracleHelper.ExecuteDataset(_connectionString, "spHACTAuthorizeUser", Convert.ToString(Token1), Convert.ToString(Token2),
                   GConvert.ToInt32(NTLoginValid));

            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                g_logger.Error(ex.Message.ToString());
            }
            //Return the records
            return dsResult;
        }

        public DataSet GetUserToken(string Token, int Condition)
        {
            DataSet dsResult = new DataSet();
            try
            {
                dsResult = (DataSet)OracleHelper.ExecuteDataset(_connectionString, "spHACTValidateToken", Convert.ToString(Token), Condition);

            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                g_logger.Error(ex.Message.ToString());
            }
            //Return the records
            return dsResult;
        }

        public DataSet InvalidateToken(string Email, int Condition)
        {
            DataSet dsResult = new DataSet();
            try
            {
                dsResult = (DataSet)OracleHelper.ExecuteDataset(_connectionString, "spHACTInValidateToken", Convert.ToString(Email), Condition);

            }
            catch (Exception ex)
            {
                //Get the User defined Error message
                g_logger.Error(ex.Message.ToString());
            }
            //Return the records
            return dsResult;
        }
        */
        #endregion

        public Models.UserInfo getUserDetails(dynamic requestParameter)
        {
            DataSet dsResult = new DataSet();
            Models.UserInfo loginLst = new Models.UserInfo();
            try
            {
                dsResult = (DataSet)OracleHelper.ExecuteDataset(_connectionString, "pkg_mofa.pr_getuserdetails", requestParameter);

                #region unused
                /*using (OracleConnection _con = new OracleConnection(_connectionString))
                {
                    _con.Open();
                    OracleCommand cmd = new OracleCommand("pkg_ideation_afkari.getUserIdeas", _con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new OracleParameter("p_ideaowner", OracleDbType.Varchar2, 30, "User1"))
                        .Value = "User1";
                    cmd.Parameters["p_ideaowner"].Direction = ParameterDirection.Input;
                    cmd.Parameters.Add(new OracleParameter("p_curideas", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new OracleParameter("p_retval", OracleDbType.Varchar2, 30, "")).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(new OracleParameter("p_message", OracleDbType.Varchar2, 30, "")).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    OracleDataAdapter da = new OracleDataAdapter(cmd);
                    da.Fill(dsResult,(OracleRefCursor)(cmd.Parameters["p_curideas"].Value));                    
                }*/
                #endregion

                if (dsResult!=null &&  dsResult.Tables.Count>0 && dsResult.Tables[0].Rows.Count>0)
                {
                    loginLst = ConvertDT<Models.UserInfo>(dsResult.Tables[0]);
                }                
            }
            catch (Exception ex)
            {              
                throw ex;
            }            
            return loginLst;
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                //g_logger.Error(T_item.ToString());
                data.Add(item);
            }
            return data;
        }

        private static T ConvertDT<T>(DataTable dt)
        {            
            T item = GetItem<T>(dt.Rows[0]);               
            return item;
        }

        private static T GetItem<T>(DataRow dr)
        {          
                Type temp = typeof(T);
                T obj = Activator.CreateInstance<T>();
            try
            {
                foreach (DataColumn column in dr.Table.Columns)
                {
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if ((pro.Name.ToUpper() == column.ColumnName.ToUpper()))
                        {
                            if (dr[column.ColumnName] == DBNull.Value)
                            {
                                if (pro.PropertyType.Equals(typeof(decimal)))
                                {
                                    dr[column.ColumnName] = 0.0;
                                }
                                else if (pro.PropertyType.Equals(typeof(string)))
                                {
                                    dr[column.ColumnName] = "";
                                }
                                else if (pro.PropertyType.Equals(typeof(DateTime)))
                                {
                                    dr[column.ColumnName] = default(DateTime);
                                }
                            }
                            if (pro.PropertyType.Equals(typeof(DateTime)))
                            {
                                pro.SetValue(obj, Convert.ToDateTime(dr[column.ColumnName]), null);
                            }
                            else
                            { pro.SetValue(obj, dr[column.ColumnName], null); }
                            break;
                        }
                        else
                            continue;
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return obj;
        }

        public Models.Response addCompany(dynamic requestParameter)
        {
            DataSet dsResult = new DataSet(); Response res = new Response();
            try
            {
                res = OracleHelper.ExecuteNonQuery(_connectionString, "pkg_mofa.PR_AddCompany", requestParameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
            return res;
        }

        public Models.Response registerCompany(dynamic requestParameter)
        {
            DataSet dsResult = new DataSet(); int result = 0;
            Response res;
            try
            {
                res = OracleHelper.ExecuteNonQuery(_connectionString, "pkg_mofa.pr_registercompany", requestParameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        public Models.Response AddAttestation(dynamic requestParameter)
        {
            DataSet dsResult = new DataSet(); int result = 0;
            Response res;
            try
            {
                res = OracleHelper.ExecuteNonQuery(_connectionString, "pkg_mofa_entity.pr_addattestationreq", requestParameter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        #region ununsedcode        
        /*public Models.AfkariModels.response postUserIdea(Models.AfkariModels.postIdea param)
        {
            Models.AfkariModels.response res = new Models.AfkariModels.response();
            try
            {
                int retval = 0;
                //res.respcode = OracleHelper.ExecuteNonQuery(_connectionString, "pkg_ideation_afkari.postIdea", requestParameter);
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();
                    cmd.CommandText = "pkg_ideation_afkari.postIdea";
                    cmd.Parameters.Add(new OracleParameter("p_ideaowner", param.p_ideaowner));
                    cmd.Parameters.Add(new OracleParameter("p_title", param.p_title));
                    cmd.Parameters.Add(new OracleParameter("p_ideadescr", param.p_ideaowner));
                    cmd.Parameters.Add(new OracleParameter("p_oldideaid", param.p_oldideaid));
                    cmd.Parameters.Add(new OracleParameter("p_ideatag", param.p_ideatag));
                    cmd.Parameters.Add(new OracleParameter("p_ideacategory", param.p_ideacategory));
                    cmd.Parameters.Add(new OracleParameter("p_status", param.p_status));
                    cmd.Parameters.Add(new OracleParameter("p_comments", param.p_comments));
                    cmd.Parameters.Add(new OracleParameter("p_ideasource", param.p_ideassource));
                    cmd.Parameters.Add(new OracleParameter("p_linemanager", param.p_linemanager));
                    cmd.Parameters.Add(new OracleParameter("p_ideatype", param.p_ideatype));
                    cmd.Parameters.Add(new OracleParameter("p_division", param.p_division));
                    cmd.Parameters.Add(new OracleParameter("p_problem", param.p_problem));
                    cmd.Parameters.Add(new OracleParameter("p_solution", param.p_solution));
                    cmd.Parameters.Add(new OracleParameter("p_relatedtrends", param.p_relatedtrends));
                    cmd.Parameters.Add(new OracleParameter("p_benefit", param.p_benefit));
                    cmd.Parameters.Add(new OracleParameter("p_benefitdesc", param.p_benefitdesc));
                    cmd.Parameters.Add(new OracleParameter("p_campaignid", param.p_campaignid));
                    cmd.Parameters.Add(new OracleParameter("p_video", param.p_video));
                    cmd.Parameters.Add(new OracleParameter("p_ideapicture", param.p_ideapicture));
                    cmd.Parameters.Add(new OracleParameter("p_attachments", param.p_attachments));
                    cmd.Parameters.Add(new OracleParameter("p_retval",OracleDbType.Int32,10,ParameterDirection.Output));
                    cmd.Parameters.Add(new OracleParameter("p_message", OracleDbType.Varchar2,ParameterDirection.Output));
                    cmd.Parameters["p_message"].Value = "";
                    cmd.Parameters["p_message"].Size = 1000;
                    cmd.ExecuteNonQuery();
                    string r = cmd.Parameters["p_retval"].Value.ToString();
                    res.respcode = Convert.ToInt32(r);
                    cmd.Parameters.Clear();
                    cmd.Dispose();
                }
                if(res.respcode == 1)
                {
                    res.message = "Success";
                    res.respStatus = res.message;
                }
            }
            catch (Exception ex)
            {
                res.respcode = 0;
                res.message = ex.Message;
                res.respStatus = "Failed";
                //Get the User defined Error message
                //g_logger.Error(ex.Message.ToString());                
                throw;
            }
            return res;
        }*/

        /*public Models.Response AddCampaign(dynamic requestParameter)
        {
            Models.Response res = new Models.Response();
            try
            {
                res.responsecode = OracleHelper.ExecuteNonQuery(_connectionString, "PKG_MOFA.PR_ADDCOMPANY", requestParameter);
                if (res.responsecode == 1)
                {
                    res.message = "Success";
                }
            }
            catch (Exception ex)
            {
                res.responsecode = 0;
                res.message = ex.Message;
                //Get the User defined Error message
                g_logger.Error(ex.Message.ToString());
            }
            return res;
        }*/
        #endregion
    }
}