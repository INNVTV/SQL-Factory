using System;
using SqlInitializer.Sql;

namespace SqlInitializer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting initializer...");

            var success = Initializer.InitializeDatabase("", "", "");

            if(success)
            {
                Console.WriteLine("Initialization complete!");
                Console.WriteLine("Exiting...");
            }
            else
            {

            }

            
        }
    }
}
