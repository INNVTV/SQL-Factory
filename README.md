# SQL Factory
A microservice for schema development, deployment and seeding of SQL databases as part of the initialization or ongoing provisioning processes of a larger platform.

A console project runs SQL scripts (grouped in batches) against a SQL database in a predefined sequence that is easy to manage, update and refactor.

In this sample both the console application and the SQL Server are hosted within Docker containers using Docker Compose.

# SQL Initializer
The current example only runs once at startup and then exists. This is useful as an "Initializer" service that creates a database one time that is required for a much larger platform to run.

# SQL Provisioning Service
This "Initializer" can easily be refactored into a "Provisioning Service" that subscribes to messages. These messages can cover scenarios such as new tenants that require on-boarding, or instances of schemas required to run a background process. Once a message comes in with a schema id the provisioner runs a new instance of the schema for that tenant on the shared database. You will also need to update the Main method to run on a while loop in order to keep the console app alive:

    static void Main(string[] args)
    {   
        while (true)
        {
            //Check for messages. Process.
            Thread.Sleep(60000); // Sleep 1 minute...
        }
    }


# Partitioning by SchemaId
The **#schema#** injection example within this project is showcased for the purpose of seperating instances of the schema by a unique namespace. Client applications can then access the table structure through this namespace - effectively seperating the database by a schemaId and creating a simple partitioning mechanism.

Alternatively a new database (or even a database server) can be provisioned per tenant - depending on your scaling and resourec requirements.

# SQL Script Management
All SQL scripts are managed within the 'Sql/Scripts' folder. The scripts are run in batches grouped within folders in the following order:

* Pre
* Tables
* Post
* Seeds

This allows you to easily manage all your SQL scripts to run in sequential batches.

The 'Sql/Statements' folder is used to run queries against the database after the batched scripts are run.

# Running this sample
This sample uses Docker Compose to build and publish the console application and the associated SQL Server instance as two separate containers. Simply run the following commands:
    
    docker-compose build
    docker-compose up

# Running against a cloud database
This sample can be easily decoupled from the containerized SQL instance by pointing your app/env settings to a SQL Azure instance running in the cloud. You will be able to run using both Docker Compose, Docker Run, or use the full debugging features available by running with dotnet:
    
    dotnet restore
    dotnet build
    dotnet run

Keep in mind that the connection string format may differ between SQL for Linux and SQL Azure. There is a variation available within the **SqlFactory.Helpers.SqlConnectionStrings** class.

# Running in production
In real workd scenarios you will want to make sure your SQL instance is fully running before running the factory as an initializer or provisioner. You can handle this by checking for resource connectivity and rechecking using an exponential back-off strategy. You can also use a message queue to signal that it is time to start up or even sleep the thread for a duration of time before running.

You may want to check if the database or particular tables exist before running certain scripts. The **SqlInitializer.Sql.Statements.VerificationStatements** class can be leveraged for just such a purpose.
