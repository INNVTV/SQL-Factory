using System;
using System.IO;
using System.Collections.Generic;
using SqlFactory.Models;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace SqlFactory
{
    public static class Manager
    {
        public static DataAccessResponse InitializeDatabase(SqlSettings sqlSettings, AppSettings appSettings)
        {
            var retryPolicy = Helpers.RetryPolicies.GetRetryPolicy();

            var createDatabaseResult = CreateDatabaseOnMaster(sqlSettings, appSettings.databaseName, retryPolicy);

            if(createDatabaseResult.isSuccess)
            {   

                var runSqlScriptsResult = RunSqlScripts(sqlSettings, appSettings, retryPolicy);

                return new DataAccessResponse{
                    isSuccess = true,
                    errorMessage = ""
                };
            }
            else{
                return new DataAccessResponse{
                    isSuccess = false,
                    errorMessage = "Failed to create database '" + appSettings.databaseName + "'"
                };
            }


        }
        
        

        private static DataAccessResponse CreateDatabaseOnMaster(SqlSettings sqlSettings, string databaseName, RetryPolicy retryPolicy)
        {
            //Connect to 'master'
            ReliableSqlConnection sqlConnectionMaster = new ReliableSqlConnection(
                Helpers.SqlConnectionStrings.GenerateConnectionString(sqlSettings, "master"),
                retryPolicy);

            //Create Database:
            var sqlStatement = new StringBuilder();
            sqlStatement.Append("IF  NOT EXISTS ");
            sqlStatement.Append("(SELECT 1 FROM sys.databases WHERE name = N'");
            sqlStatement.Append(databaseName);
            sqlStatement.Append("') ");
            sqlStatement.Append("BEGIN ");
            sqlStatement.Append("CREATE DATABASE ");
            sqlStatement.Append(databaseName);
            sqlStatement.Append(" END");

            SqlCommand sqlCommand = sqlConnectionMaster.CreateCommand();
			sqlCommand.CommandText = sqlStatement.ToString();
            sqlCommand.Connection.OpenWithRetry();
            int result = sqlCommand.ExecuteNonQueryWithRetry(); // returns Int indicating number of rows affected
            sqlCommand.Connection.Close();

            return new DataAccessResponse { isSuccess = true };
        }

        private static DataAccessResponse RunSqlScripts(SqlSettings sqlSettings, AppSettings appSettings, RetryPolicy retryPolicy)
        {
            //Connect to '<databaseName>'
                ReliableSqlConnection sqlConnection = new ReliableSqlConnection(
                    Helpers.SqlConnectionStrings.GenerateConnectionString(sqlSettings, appSettings.databaseName),
                    retryPolicy);


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
                            var script = s.Replace("#schema#", appSettings.schemaName);

                            Helpers.SqlExecution.ExecuteNonQueryStatement(script, sqlConnection);
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