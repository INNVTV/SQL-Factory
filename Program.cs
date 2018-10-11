using System;
using System.IO;
using SqlInitializer.Models;
using System.Threading;

namespace SqlInitializer
{
    class Program
    {
        static void Main(string[] args)
        {   
            Console.WriteLine("Starting initializer...");
       
            var appSettings = Settings.GetAppSettings();
            var sqlSettings = Settings.GetSqlSettings();

            var response = Manager.InitializeDatabase(sqlSettings, appSettings);          

            if(response.isSuccess)
            {
                Console.WriteLine("Initialization complete!");
                //Send message to other microservices...
                
                Console.WriteLine("Exiting...");
            }
            else
            {
                Console.WriteLine(response.errorMessage);
            }          
        }     
    }
}
