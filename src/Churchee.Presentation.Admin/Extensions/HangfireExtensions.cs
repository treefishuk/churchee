using Microsoft.Data.SqlClient;

namespace Hangfire
{
    internal static class HangfireExtensions
    {
        internal static IGlobalConfiguration CreateDatabaseIfNotExists(this IGlobalConfiguration globalConfirguration, string connectionString)
        {
            var csBuilder = new SqlConnectionStringBuilder(connectionString);

            string hangfireDBName = csBuilder.InitialCatalog;

            csBuilder.InitialCatalog = "Master";

            using (var connection = new SqlConnection(csBuilder.ConnectionString))
            {
                connection.Open();

                using var command = new SqlCommand(string.Format(
                    @"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}') 
                                    CREATE DATABASE [{0}];
                      ", hangfireDBName), connection);

                command.ExecuteNonQuery();
            }

            return globalConfirguration;
        }

    }
}
