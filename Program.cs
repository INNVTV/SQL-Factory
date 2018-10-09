using System;
using SqlInitializer.Sql;
using SqlInitializer.Models;

namespace SqlInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting initializer...");

            var sqlSettings = new SqlSettings{server = "", userName = "", password = ""};

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
    }
}
