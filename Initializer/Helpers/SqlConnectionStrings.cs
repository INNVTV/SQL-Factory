using System;
using System.Text;
using SqlInitializer.Models;

namespace SqlInitializer.Initializer.Helpers
{
    public class SqlConnectionStrings
    {
        public static string GenerateConnectionString(SqlSettings sqlSettings, string databaseName)
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