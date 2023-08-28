using System;
using System.Collections;
using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace MOFAAPI.Helpers
{
    public class OracleHelper
    {
        private OracleHelper() { }
        private static void AttachParameters(OracleCommand command, OracleParameter[] commandParameters)
        {
            foreach (OracleParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }                
                command.Parameters.Add(p);
            }
        }
        private static void AssignParameterValues(OracleParameter[] commandParameters, object[] parameterValues,int noOfOutParams=0)
        {            
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                try
                {
                    commandParameters[i].Value = parameterValues[i];//parameterValues[i];
                }
                catch (Exception e) { };
            }
        }
        private static void PrepareCommand(OracleCommand command, OracleConnection connection, OracleTransaction transaction, CommandType commandType, string commandText, OracleParameter[] commandParameters)
        {          
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }         
            command.Connection = connection;            
            command.CommandText = Common.SanitizeString(commandText);            
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            //set the command type
            command.CommandType = commandType;
            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }
        public static Models.Response ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, DataTable multicommands)
        {
            int intReturn = 0;
            Models.Response res = new Models.Response();
            //create & open a SqlConnection, and dispose of it after we are done.
            try
            {
                OracleParameter[] spParamnames = SqlHelperParameterCache.GetSpParameterSet(connectionString, Common.SanitizeString(commandText));
                {
                    using (OracleConnection cn = new OracleConnection(connectionString))
                    {
                        cn.Open();
                        int intTotCol = multicommands.Columns.Count;

                        foreach (DataRow dsrow in multicommands.Rows)
                        {
                            for (int colcount = 0; colcount < intTotCol; colcount++)
                            {
                                spParamnames[colcount].Value = dsrow[colcount];
                            }
                            //call the overload that takes a connection in place of the connection string
                            res = ExecuteNonQuery(cn, commandType, Common.SanitizeString(commandText), spParamnames);
                        }
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception exp)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw exp;
            }
            return res;
        }
        public static Models.Response ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            {
                //create & open a SqlConnection, and dispose of it after we are done.
                using (OracleConnection cn = new OracleConnection(connectionString))
                {
                    cn.Open();
                    //call the overload that takes a connection in place of the connection string
                    return ExecuteNonQuery(cn, commandType, Common.SanitizeString(commandText), commandParameters);
                }
            }
        }
        public static Models.Response ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static Models.Response ExecuteNonQuery(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            Models.Response res = new Models.Response();
            using (OracleCommand cmd = new OracleCommand())
            {
                cmd.CommandTimeout = 1800;
                PrepareCommand(cmd, connection, (OracleTransaction)null, commandType, Common.SanitizeString(commandText), commandParameters);

                //finally, execute the command.
                cmd.ExecuteNonQuery();
                int retval = Convert.ToInt32(cmd.Parameters["p_retval"].Value);
                res.responsecode = retval;
                res.message = Convert.ToString(cmd.Parameters["p_retstatus"].Value);

                // detach the SqlParameters from the command object, so they can be used again.
                cmd.Parameters.Clear();
                //return retval;
                return res;
            }
        }

        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            try
            {
                {
                    using (OracleConnection cn = new OracleConnection(connectionString))
                    {
                        cn.Open();
                        //call the overload that takes a connection in place of the connection string
                        return ExecuteDataset(cn, commandType, Common.SanitizeString(commandText), commandParameters);
                    }
                }
            }
            catch(Exception e) { throw e; }
        }
        public static DataSet ExecuteDataset(string connectionString, string spName,params object[] parameterValues)
        {
            try
            {
                {
                    //if we receive parameter values, we need to figure out where they go
                    if ((parameterValues != null) && (parameterValues.Length > 0))
                    {
                        //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                        OracleParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                        //assign the provided values to these parameters based on parameter order                    
                        AssignParameterValues(commandParameters, parameterValues, 3);

                        //call the overload that takes an array of SqlParameters
                        return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                    }
                    //otherwise we can just call the SP without params
                    else
                    {
                        return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
                    }
                }
            }
            catch(Exception e)
            { throw e; }
        }

        public static DataSet ExecuteDataset(OracleConnection connection, CommandType commandType, string commandText, params OracleParameter[] commandParameters)
        {
            DataSet ds = new DataSet();
            {
                try
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.CommandTimeout = 1800;
                        PrepareCommand(cmd, connection, null, commandType, Common.SanitizeString(commandText), commandParameters);
                        //create the DataAdapter & DataSet
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            //fill the DataSet using default values for DataTable names, etc.                            
                            da.Fill(ds);
                            // detach the SqlParameters from the command object, so they can be used again.
                            cmd.Parameters.Clear();
                        }
                    }
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            //return the dataset
            return ds;
        }

        public sealed class SqlHelperParameterCache
        {
            //*********************************************************************
            //
            // Since this class provides only static methods, make the default constructor private to prevent
            // instances from being created with "new SqlHelperParameterCache()".
            //
            //*********************************************************************
            private SqlHelperParameterCache() { }
            private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
            private static OracleParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
            {
                {
                    using (OracleConnection cn = new OracleConnection(connectionString))
                    {
                        using (OracleCommand cmd = new OracleCommand(spName, cn))
                        {
                            cn.Open();
                            cmd.CommandType = CommandType.StoredProcedure;

                            OracleCommandBuilder.DeriveParameters(cmd);

                            /*if (!includeReturnValueParameter)
                            {
                                cmd.Parameters.RemoveAt(0);
                            }*/                           
                            OracleParameter[] discoveredParameters = new OracleParameter[cmd.Parameters.Count]; ;
                            cmd.Parameters.CopyTo(discoveredParameters, 0);
                            cmd.Parameters["p_retstatus"].Value = "";
                            cmd.Parameters["p_retval"].Value = "";                            
                            return discoveredParameters;
                        }
                    }
                }
            }
            private static OracleParameter[] CloneParameters(OracleParameter[] originalParameters)
            {
                //deep copy of cached SqlParameter array
                OracleParameter[] clonedParameters = new OracleParameter[originalParameters.Length];

                for (int i = 0, j = originalParameters.Length; i < j; i++)
                {
                    clonedParameters[i] = (OracleParameter)((ICloneable)originalParameters[i]).Clone();
                }

                return clonedParameters;
            }
            public static void CacheParameterSet(string connectionString, string commandText, params OracleParameter[] commandParameters)
            {
                string hashKey = connectionString + ":" + Common.SanitizeString(commandText);

                paramCache[hashKey] = commandParameters;
            }
            public static OracleParameter[] GetCachedParameterSet(string connectionString, string commandText)
            {
                string hashKey = connectionString + ":" + Common.SanitizeString(commandText);

                OracleParameter[] cachedParameters = (OracleParameter[])paramCache[hashKey];

                if (cachedParameters == null)
                {
                    return null;
                }
                else
                {
                    return CloneParameters(cachedParameters);
                }
            }
            public static OracleParameter[] GetSpParameterSet(string connectionString, string spName)
            {
                return GetSpParameterSet(connectionString, spName, false);
            }
            public static OracleParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
            {
                string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

                OracleParameter[] cachedParameters;

                cachedParameters = (OracleParameter[])paramCache[hashKey];

                if (cachedParameters == null)
                {
                    cachedParameters = (OracleParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter));
                }

                return CloneParameters(cachedParameters);
            }
        }
    }
}