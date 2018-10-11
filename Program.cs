using System;
using System.IO;
using SqlFactory.Models;
using System.Threading;

namespace SqlFactory
{
    class Program
    {
        static void Main(string[] args)
        {   
            Console.WriteLine("Starting factory...");
       
            var appSettings = Settings.GetAppSettings();
            var sqlSettings = Settings.GetSqlSettings();

            var response = Manager.InitializeDatabase(sqlSettings, appSettings);          

            if(response.isSuccess)
            {
                Console.WriteLine("Factory complete!");
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
