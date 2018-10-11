using System;
using System.Data.SqlClient;
using SqlInitializer.Models;

namespace SqlInitializer.Sql.Statements
{
    public static class VerificationStatements
    {
        public static bool DatabaseExists(SqlSettings sqlSettings, AppSettings appSettings)
        {
            bool exists = false;

            string SqlStatement =
                 "IF EXISTS (SELECT name FROM master.sys.databases WHERE name = N'" + appSettings.databaseName + "') SELECT 'true' ELSE SELECT 'false'";

            SqlCommand sqlCommand = new SqlCommand(SqlStatement.ToString(), new SqlConnection(SqlInitializer.Initializer.Helpers.SqlConnectionStrings.GenerateConnectionString(sqlSettings, "master")));
            sqlCommand.Connection.Open();
            exists = Convert.ToBoolean(sqlCommand.ExecuteScalar());

            sqlCommand.Connection.Close();

            return exists;
        }

        public static bool TableExists(string databaseName, string tableName, SqlSettings sqlSettings)
        {
            bool exists = false;

            string SqlStatement =
                "IF OBJECT_ID ('dbo." + tableName + "') IS NOT NULL SELECT 'true' ELSE SELECT 'false'";

            SqlCommand sqlCommand = new SqlCommand(SqlStatement.ToString(), new SqlConnection(SqlInitializer.Initializer.Helpers.SqlConnectionStrings.GenerateConnectionString(sqlSettings, databaseName)));
            sqlCommand.Connection.Open();
            exists = Convert.ToBoolean(sqlCommand.ExecuteScalar());

            sqlCommand.Connection.Close();

            return exists;
        }
    }
}