using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechMovesLogistics.Api.Controllers;
using TechMoves_Logistics.Data;

namespace TechMovesLogistics.Tests.Integration
{
    // Boots the API in-process for integration tests (LocalDB: TechMovesLogisticsDB_Test).
    public class ApiWebApplicationFactory : WebApplicationFactory<ContractsController>
    {
        private static readonly object DatabaseLock = new();
        private static bool _databaseInitialized;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);
            InitializeDatabase(host);
            return host;
        }

        private static void InitializeDatabase(IHost host)
        {
            lock (DatabaseLock)
            {
                if (_databaseInitialized)
                    return;

                using var scope = host.Services.CreateScope();
                ResetDatabase(scope.ServiceProvider);
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
                _databaseInitialized = true;
            }
        }

        // Drops the test database via master so parallel hosts do not race EnsureDeleted/EnsureCreated.
        private static void ResetDatabase(IServiceProvider services)
        {
            var connectionString = services.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not configured.");

            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = $"""
                    IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'{databaseName}')
                    BEGIN
                        ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        DROP DATABASE [{databaseName}];
                    END
                    """;
                command.ExecuteNonQuery();
            }
        }
    }

    [CollectionDefinition("Integration")]
    public class IntegrationTestCollection : ICollectionFixture<ApiWebApplicationFactory>
    {
    }
}
