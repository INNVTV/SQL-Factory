using System;
using System.Collections.Generic;
using SqlInitializer.Models;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace SqlInitializer.Sql
{
    public static class Initializer
    {
        public static DataAccessResponse InitializeDatabase(SqlSettings sqlSettings, string databaseName)
        {
            //Set retry policy
            var retryPolicy = new RetryPolicy<DefaultRetryStrategy>(5, new TimeSpan(0, 0, 3));

            var retryInterval = TimeSpan.FromSeconds(3);
            var strategy = new FixedInterval("fixed", 4, retryInterval);
            var strategies = new List<RetryStrategy> { strategy };
            var manager = new RetryManager(strategies, "fixed");
            RetryManager.SetDefault(manager);


            //Create Database:
            var sqlStatement = new StringBuilder();
            sqlStatement.Append("Create Database ");
            sqlStatement.Append(databaseName);

            ReliableSqlConnection sqlConnection = new ReliableSqlConnection(
                _generateConnectionString(sqlSettings, "master")
                , retryPolicy);

            SqlCommand sqlCommand = sqlConnection.CreateCommand();
			sqlCommand.CommandText = sqlStatement.ToString();

            sqlCommand.Connection.OpenWithRetry();
            int result = sqlCommand.ExecuteNonQueryWithRetry(); // returns Int indicating number of rows affected
            sqlCommand.Connection.Close();

            return new DataAccessResponse{
                isSuccess = true,
                errorMessage = ""
            };
        }

        public class DefaultRetryStrategy : ITransientErrorDetectionStrategy
        {
            public bool IsTransient(Exception ex)
            {
                if (ex != null && ex is SqlException)
                {
                    foreach (SqlError error in (ex as SqlException).Errors)
                    {
                        switch (error.Number)
                        {
                            case 1205:
                                //System.Diagnostics.Debug.WriteLine("SQL Error: Deadlock condition. Retrying...");
                                return true;

                            case -2:
                                //System.Diagnostics.Debug.WriteLine("SQL Error: Timeout expired. Retrying...");
                                return true;
                        }
                    }
                }
                // For all others, do not retry.
                return false;
            }
        }

        private static string _generateConnectionString(SqlSettings sqlSettings, string databaseName)
        {
            StringBuilder connectionString = new StringBuilder();

            //SQL Linux/Docker Connection String: --------------
            connectionString.Append("Server=");
            connectionString.Append(sqlSettings.server);

            connectionString.Append(";Database=");
            connectionString.Append(databaseName);

            connectionString.Append(";User=");
            connectionString.Append(sqlSettings.userName);

            connectionString.Append(";Password=");
            connectionString.Append(sqlSettings.password);

            // SQL Azure Connection String: ---------------
            /*
            connectionString.Append("Server=");
            connectionString.Append(sqlSettings.server);

            connectionString.Append(";Database=");
            connectionString.Append(databaseName);

            connectionString.Append(";User Id=");
            connectionString.Append(sqlSettings.userName);

            connectionString.Append(";Password=");
            connectionString.Append(sqlSettings.password);
            connectionString.Append(";MultipleActiveResultSets=true");
            connectionString.Append(";Trusted_Connection=False;Encrypt=True;Persist Security Info=True;"); //<-- Adding Persist Security Info=True; resolved SQL connectivity errors when making multiple calls
            connectionString.Append("Connection Timeout=45;"); //<-- Tripled the default timeout
            */

            return connectionString.ToString();
        }
    }
}