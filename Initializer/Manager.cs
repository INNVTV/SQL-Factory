using System;
using System.IO;
using System.Collections.Generic;
using SqlInitializer.Models;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace SqlInitializer
{
    public static class Manager
    {
        public static DataAccessResponse InitializeDatabase(SqlSettings sqlSettings, string databaseName)
        {
            var retryPolicy = Initializer.Helpers.RetryPolicies.GetRetryPolicy();

            var createDatabaseResult = CreateDatabaseOnMaster(sqlSettings, databaseName, retryPolicy);

            if(createDatabaseResult.isSuccess)
            {   

                var runSqlScriptsResult = RunSqlScripts(sqlSettings, databaseName, retryPolicy);

                return new DataAccessResponse{
                    isSuccess = true,
                    errorMessage = ""
                };
            }
            else{
                return new DataAccessResponse{
                    isSuccess = false,
                    errorMessage = "Failed to create database '" + databaseName + "'"
                };
            }


        }
        
        

        private static DataAccessResponse CreateDatabaseOnMaster(SqlSettings sqlSettings, string databaseName, RetryPolicy retryPolicy)
        {
            //Connect to 'master'
            ReliableSqlConnection sqlConnectionMaster = new ReliableSqlConnection(
                Initializer.Helpers.SqlConnectionStrings.GenerateConnectionString(sqlSettings, "master"),
                retryPolicy);

            //Create Database:
            //TODO: Check if database exists!
            var sqlStatement = new StringBuilder();
            sqlStatement.Append("Create Database ");
            sqlStatement.Append(databaseName);

            SqlCommand sqlCommand = sqlConnectionMaster.CreateCommand();
			sqlCommand.CommandText = sqlStatement.ToString();
            sqlCommand.Connection.OpenWithRetry();
            int result = sqlCommand.ExecuteNonQueryWithRetry(); // returns Int indicating number of rows affected
            sqlCommand.Connection.Close();

            //Console.WriteLine(result);
            //TODO: Better way to check if db created

            return new DataAccessResponse { isSuccess = true };
        }

        private static DataAccessResponse RunSqlScripts(SqlSettings sqlSettings, string databaseName, RetryPolicy retryPolicy)
        {
            //Connect to '<databaseName>'
                ReliableSqlConnection sqlConnection = new ReliableSqlConnection(
                    Initializer.Helpers.SqlConnectionStrings.GenerateConnectionString(sqlSettings, databaseName),
                    retryPolicy);

                //Use a schema name (can be injected by container or used in provisioning scripts)
                var schemaName = "tenant1001";

                //List of script folders to be run (in order)
                var scriptsOrder = new List<string>();
                scriptsOrder.Add("Pre");
                scriptsOrder.Add("Tables");
                scriptsOrder.Add("Post");
                scriptsOrder.Add("Seeds");

                //Loop through all scripts within each folder and run them against the database connection:
                foreach (string folder in scriptsOrder)
                {
                    var files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/Sql/Scripts/" + folder);
                    foreach(string file in files)
                    {
                        Console.WriteLine("Running script: '" + file + "'");

                        var fileData = System.IO.File.ReadAllText(file);

                        var split = SplitSqlStatements(fileData);
                        foreach (var s in split)
                        {
                            //REPLACE [schemaname] with schemaName/tenantId/accountID:
                            var script = s.Replace("#schema#", schemaName);

                            Console.WriteLine(s);
                            Initializer.Helpers.SqlExecution.ExecuteNonQueryStatement(s, sqlConnection);
                        }
                    }
                }

                return new DataAccessResponse { isSuccess = true };
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