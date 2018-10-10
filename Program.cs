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
            //Sleep to allow SQL Container time to start up
            Thread.Sleep(20000);
            //TODO: Make it check and retry until resource ready.

            Console.WriteLine("Starting initializer...");
       
            var sqlSettings = Settings.GetSqlSettings();
            var response = Manager.InitializeDatabase(sqlSettings, "Initializer");          

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
