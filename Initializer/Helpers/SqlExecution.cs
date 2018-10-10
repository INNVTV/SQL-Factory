using System;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace SqlInitializer.Initializer.Helpers
{
    public static class SqlExecution
    {
        public static void ExecuteNonQueryStatement(string statement, ReliableSqlConnection sqlConnection)
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
                catch(Exception e)
                {
                    Console.WriteLine("ExecuteNonQueryException: '" + e.Message + "'");
                    Console.WriteLine("ADO.NET Connection Pooling may require a retry.");
                    Console.WriteLine("Trying again....");

                    //Try again (ADO.NET Connection Pooling may require a retry)
                    sqlCommandGenerateTables.Connection.Close();

                    sqlCommandGenerateTables.Connection.OpenWithRetry();
                    sqlCommandGenerateTables.ExecuteNonQueryWithRetry();
                    sqlCommandGenerateTables.Connection.Close();
                }
            }
        }
    }
}