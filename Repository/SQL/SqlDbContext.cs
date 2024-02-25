using ElevatorAction.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandType = System.Data.CommandType;
using System.Runtime.CompilerServices;

namespace ElevatorAction.Repository.SQL
{
    public class SqlDbContext : IDisposable
    {
        #region PRIVATE PROPERTIES

        private ILogger _log;
        private SqlConnection _connection;
        private int? _timeOut;

        #endregion PRIVATE PROPERTIES

        #region PUBLIC PROPERTIES
        #endregion PUBLIC PROPERTIES


        #region CONSTRUCTORS/DESTRUCTORS
        public SqlDbContext(string connectionString, ILogger logger, int? timeOut = null)
        {
            _connection = new SqlConnection(connectionString);
            _log = logger;

            _timeOut = timeOut;
        }

        public void Dispose()
        {
            CloseConnection();
            _connection.Dispose();
        }
        #endregion CONSTRUCTORS/DESTRUCTORS


        #region PRIVATE METHODS

        private string attributedErrorMessage(string spName, string errorMessage, [CallerMemberName] string caller = "")
        {
            return $"SQL ERROR in SqlDbContext.{caller} executing SP '{spName}': {errorMessage}";
        }


        /// <summary>
        /// Set a SqlCommand's database parameters based on a dictionary of parameter objects
        /// </summary>
        /// <param name="command">The command on which to set the parameters</param>
        /// <param name="parameters">A dictionary of keys (parameter names) and values to send with the command to the db</param>
        private void setDbParameters(SqlCommand command, Dictionary<string, object> parameters)
        {
            if ((parameters?.Count ?? 0) > 0)
                foreach (var param in parameters)
                {
                    if (param.Value != null)
                        command.Parameters.Add(new SqlParameter(param.Key, param.Value));
                }
        }

        /// <summary>
        /// This method reads one database record and serializes it into a json-like string ready for deserialization into an object class
        /// </summary>
        /// <param name="rdr">Sql Data Reader with db data being read</param>
        /// <returns>String that holds json-like serialized data from database</returns>
        private string serializeDbRecord(SqlDataReader rdr)
        {
            if ((rdr == null) || (rdr.FieldCount == 0))
                return "";

            var sb = new StringBuilder("{");
            for (int i = 0; i < rdr.FieldCount; i++)
            {
                if (rdr[i] == null)
                    continue;

                sb.Append("\"").Append(rdr.GetName(i)).Append("\": ")
                    .Append("\"").Append(rdr[i]).Append("\",");
            }
            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Execute SP and returns records, creating and destroying its own SqlConnection for this DB operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command to execute on the database</param>
        /// <param name="parameters">Parameters to be passed to the SP</param>
        /// <returns>Returns a list of records from the executed SP, deserialized into the specified object type</returns>
        private Response<List<T>> getSqlResults<T>(SqlCommand command) where T : new()
        {
            var result = new List<T>();

            try
            {
                using (command)
                {
                    // execute the command
                    using (SqlDataReader rdr = command.ExecuteReader())
                    {
                        // iterate through results, deserializing each into the object to return
                        while (rdr.Read())
                        {
                            try
                            {
                                string thisItemStr = serializeDbRecord(rdr);
                                result.Add(JsonConvert.DeserializeObject<T>(thisItemStr));
                            }
                            catch (Exception ex)
                            {
                                _log.LogError(attributedErrorMessage(command?.CommandText, ex.Message));
                            }
                        }
                    }
                }

                _log.LogInformation($"SQL read {result.Count} records from SP {command?.CommandText}");

                return new Response<List<T>>(result);
            }
            catch (Exception ex)
            {
                _log.LogError(attributedErrorMessage(command?.CommandText, ex.Message));
                return new Response<List<T>>(false, ex.Message);
            }
        }

        private SqlCommand CreateSPCommand(string storedProcName, bool openConnection = true)
        {
            var command = new SqlCommand(storedProcName, _connection);
            command.CommandType = CommandType.StoredProcedure;

            if (_timeOut.HasValue)
                command.CommandTimeout = _timeOut.Value;

            if (openConnection)
                OpenConnection();

            return command;
        }

        private Response<List<T>> validateSqlListResponse<T>(Response<List<T>> resultList)
        {
            var errMsg = "No data returned from database";

            if (!(resultList?.Success ?? false))
                return new Response<List<T>>(false, resultList?.Message ?? errMsg);

            if (resultList.Data is null || resultList.Data.Count() == 0)
                return new Response<List<T>>(false, errMsg);

            return resultList;
        }

        #endregion PRIVATE METHODS


        #region PUBLIC METHODS

        /// <summary>
        /// If the SQL connection is closed, open it
        /// </summary>
        public void OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

        /// <summary>
        /// If the SQL connection is closed, close it
        /// </summary>
        public void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
        }

        /// <summary>
        /// Initialize call to getSqlResults to return list of records
        /// </summary>
        /// <typeparam name="T">Specify the type - a list of this type will be deserialized from the db and returned</typeparam>
        /// <param name="storedProcName">excecuted SP</param>
        /// <param name="parameters"></param>
        /// <returns>A list of records</returns>
        public Response<List<T>> GetSqlResultListFromSP<T>(string storedProcName, Dictionary<string, object> parameters = null) where T : new()
        {
            var command = CreateSPCommand(storedProcName);
            setDbParameters(command, parameters);

            var resultList = getSqlResults<T>(command);
            return validateSqlListResponse(resultList);
        }

        /// <summary>
        /// Initialize call to getSqlResults to return records matching the query, but select the top record to return a single object
        /// </summary>
        /// <typeparam name="T">Specify the type - an object of this type will be deserialized from the db and returned</typeparam>
        /// <param name="storedProcName">excecuted SP</param>
        /// <param name="parameters"></param>
        /// <returns>A list of records</returns>
        public Response<T> GetSqlResultFromSP<T>(string storedProcName, Dictionary<string, object> parameters = null) where T : new()
        {
            var command = CreateSPCommand(storedProcName);
            setDbParameters(command, parameters);

            var resultList = getSqlResults<T>(command);
            var validatedResponse = validateSqlListResponse<T>(resultList);

            if (validatedResponse.Success)
                return new Response<T>(validatedResponse.Data.FirstOrDefault());
            else
                return new Response<T>(false, "No data returned from database");
        }

        /// <summary>Do a non-query DB operation (eg update, delete), and receive back a count of affected rows</summary>
        /// <param name="storedProcName">Name of the stored procedure to be executed</param>
        /// <param name="parameters">Parameters to be passed into the stored procedure</param>
        /// <returns>Count of rows affected</returns>
        public Response<int> ExecuteNonQuerySP(SqlConnection connection, string storedProcName, Dictionary<string, object> parameters = null, SqlTransaction currentTransaction = null)
        {
            try
            {
                var command = CreateSPCommand(storedProcName);
                setDbParameters(command, parameters);

                using (command)
                {
                    var rowCount = command.ExecuteNonQuery();
                    return new Response<int>(rowCount);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(attributedErrorMessage(storedProcName, ex.Message));
                return new Response<int>(false, ex.Message);
            }
        }

        /// <summary>Do a scalar DB operation (eg create). Use for create procedures to get the created ID back.</summary>
        /// <param name="storedProcName">Name of the stored procedure to be executed</param>
        /// <param name="parameters">Parameters to be passed into the stored procedure</param>
        /// <returns>Returns a scalar (eg. int or string) response from a stored procedure</returns>
        public Response<object> GetSqlScalarValueFromSP(string storedProcName, Dictionary<string, object> parameters = null)
        {
            var command = CreateSPCommand(storedProcName);
            try
            {
                setDbParameters(command, parameters);

                using (command)
                {
                    var returnValue = command.ExecuteScalar();
                    return new Response<object>(returnValue);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(attributedErrorMessage(storedProcName, ex.Message));
                return new Response<object>(false, ex.Message);
            }
        }

        #endregion PUBLIC METHODS
    }
}
