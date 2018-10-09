using SqlInitializer.Models;

namespace SqlInitializer.Sql
{
    public static class Initializer
    {
        public static DataAccessResponse InitializeDatabase(SqlSettings sqlSettings)
        {
            return new DataAccessResponse{
                isSuccess = true,
                errorMessage = ""
            };
        }
    }
}