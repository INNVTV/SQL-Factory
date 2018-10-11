namespace SqlInitializer.Models
{
    public class AppSettings
    {
        public string databaseName;
        public string schemaName; //<-- can be injected by container or used in incoming message queues if refactoring for provisioning services.

    }
}