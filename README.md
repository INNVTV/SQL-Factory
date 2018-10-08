# SQL-Initializer
Microservice for schema development, deployment and seeding of SQL databases as part of the initialization or provisioning process of a larger platform.

A console project runs SQL scripts (grouped in batches) against a SQL database in a sequence that includes pre-scripts, tables, seeds and post scripts.

In this sample application the SQL Server is hosted within a Docker container.

# SQL Script Management
All SQL scripts are managed within the 'sql/scripts' folder. The scripts are run in batches grouped within folders in the following order:

* pre
* tables
* seeds
* post

This allows you to easily manage all your SQL scripts in sequential batches.

The 'sql/statements' folder is used to run queries against the database after the batched scripts are run.

# Running sample
This sample uses Docker Compose to build and publish the console application and the associated SQL Server instance as two seperate instances. Simply run the following command:
    
    docker-compose up


