using System;
using System.IO;
using System.Collections.Generic;
using SqlInitializer.Models;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System.Reflection;

namespace SqlInitializer.Sql
{
    public static class Initializer
    {
        public static DataAccessResponse InitializeDatabase(SqlSettings sqlSettings, string databaseName)
        {
            //Set retry policy
            /* 
            var retryPolicy = new RetryPolicy<DefaultRetryStrategy>(5, new TimeSpan(0, 0, 3));

            var retryInterval = TimeSpan.FromSeconds(3);
            var strategy = new FixedInterval("fixed", 4, retryInterval);
            var strategies = new List<RetryStrategy> { strategy };
            var manager = new RetryManager(strategies, "fixed");
            RetryManager.SetDefault(manager);

            //Connect to 'master'
            ReliableSqlConnection sqlConnection = new ReliableSqlConnection(
                _generateConnectionString(sqlSettings, "master")
                , retryPolicy);

            //Create Database:
            var sqlStatement = new StringBuilder();
            sqlStatement.Append("Create Database ");
            sqlStatement.Append(databaseName);

            SqlCommand sqlCommand = sqlConnection.CreateCommand();
			sqlCommand.CommandText = sqlStatement.ToString();
            sqlCommand.Connection.OpenWithRetry();
            int result = sqlCommand.ExecuteNonQueryWithRetry(); // returns Int indicating number of rows affected
            sqlCommand.Connection.Close();

            if(result > 0)
            {*/
                //var assembly = Assembly.GetAssembly(GetType());
                var assembly = Assembly.GetAssembly(typeof(Assembly));
                var assemblyName = assembly.GetName().Name;

                var resourceNames = assembly.GetManifestResourceNames();

                //List of script folders to be run (in order)
                var scriptsOrder = new List<string>();
                scriptsOrder.Add("Pre");
                scriptsOrder.Add("Tables");
                scriptsOrder.Add("Post");
                scriptsOrder.Add("Procedures");
                scriptsOrder.Add("Seed");

                //Loop through all scripts within each folder and run them against the database connection:
                //FYI: .sql Files must be saved as ANSI
                //FYI: .sql Files must be set as "Embedded Resource" & "CopyAlways" in Properties
                foreach (string folder in scriptsOrder)
                {
                    Console.WriteLine(assemblyName);

                    foreach (var sqlScript in resourceNames.Where(o => o.StartsWith(assemblyName + "SqlInitializer.Sql.Scripts." + folder)))
                    {                      
                        using (var stream = assembly.GetManifestResourceStream(sqlScript))
                        using (var reader = new StreamReader(stream))
                        {
                            var split = SplitSqlStatements(reader.ReadToEnd());
                            Console.WriteLine(split);

                            foreach (var s in split)
                            {
                                //executeNonQueryStatement(s, sqlConnection);
                            }
                        }
                    }
                }

                return new DataAccessResponse{
                    isSuccess = true,
                    errorMessage = ""
                };
            //}
            //else{
                //return new DataAccessResponse{
                    //isSuccess = false,
                    //errorMessage = "Failed to create database '" + databaseName + "'"
                //};
           // }


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

        private static void executeNonQueryStatement(string statement, ReliableSqlConnection sqlConnection)
        {
            if (statement != "")
            {

                //SqlCommand sqlCommandGenerateTables = new SqlCommand(statement, sqlConnection);
                SqlCommand sqlCommandGenerateTables = sqlConnection.CreateCommand();
			    sqlCommandGenerateTables.CommandText = statement;
			    

                try
                {

                    sqlCommandGenerateTables.Connection.OpenWithRetry();
                    sqlCommandGenerateTables.ExecuteNonQueryWithRetry();
                    sqlCommandGenerateTables.Connection.Close();
                }
                catch
                {
                    //Try again (ADO.NET Connection Pooling may require a retry)
                    sqlCommandGenerateTables.Connection.Close();

                    sqlCommandGenerateTables.Connection.OpenWithRetry();
                    sqlCommandGenerateTables.ExecuteNonQueryWithRetry();
                    sqlCommandGenerateTables.Connection.Close();
                }
            }

        }


        static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements
            var statements = System.Text.RegularExpressions.Regex.Split(
                    sqlScript,
                    @"^\s*GO\s* ($ | \-\- .*$)",
                    System.Text.RegularExpressions.RegexOptions.Multiline |
                    System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace |
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
        }
    }
}