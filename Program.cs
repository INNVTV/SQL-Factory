using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using SqlInitializer.Sql;
using SqlInitializer.Models;

namespace SqlInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting initializer...");
       
            var sqlSettings = GetSqlSettings();
            var response = Initializer.InitializeDatabase(sqlSettings);          

            if(response.isSuccess)
            {
                Console.WriteLine("Initialization complete!");
                Console.WriteLine("Exiting...");
            }
            else
            {
                Console.WriteLine(response.errorMessage);
            }         
        }

        public static SqlSettings GetSqlSettings()
        {
            //Get configuration from Docker (via appsettings.json)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables(); //<-- Allows for Docker Env Variables

            IConfigurationRoot configuration = builder.Build();

            var _server = configuration["Sql:Server"];
            var _userName = configuration["Sql:UserName"];
            var _password = configuration["Sql:Password"];

            var sqlSettings = new SqlSettings{
                    server = _server,
                    userName = _userName,
                    password = _password
                };

            return sqlSettings;
        }
    }
}
