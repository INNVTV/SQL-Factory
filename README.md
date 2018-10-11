# SQL-Initializer
Microservice for schema development, deployment and seeding of SQL databases as part of the initialization or provisioning process of a larger platform.

A console project runs SQL scripts (grouped in batches) against a SQL database in a sequence that includes pre-scripts, tables, seeds and post scripts.

In this sample application the SQL Server is hosted within a Docker container.

# Provisioning Service
The "Initializer" can easily be refactored into a "Provisioning Service" that subscribes to messages regarding new tenants that require on-boarding. Onec a message comes in with a schema id the provisioner runs a new instance of the schema for that tenant on the shared database. 

The #schema# injection example within this project is showcased here for that very purpose.

ALternatively a new database (or even a database server) can be provisioned per tenant - depending on your application scaling and resourec requirements.

# SQL Script Management
All SQL scripts are managed within the 'Sql/Scripts' folder. The scripts are run in batches grouped within folders in the following order:

* Pre
* Tables
* Post
* Seeds

This allows you to easily manage all your SQL scripts to run in sequential batches.

The 'Sql/Statements' folder is used to run queries against the database after the batched scripts are run.

# Running a sample
This sample uses Docker Compose to build and publish the console application and the associated SQL Server instance as two seperate instances. Simply run the following command:
    
    docker-compose up


