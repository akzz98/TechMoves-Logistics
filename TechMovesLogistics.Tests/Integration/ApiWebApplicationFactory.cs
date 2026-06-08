using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TechMoves_Logistics.Data;

namespace TechMovesLogistics.Tests.Integration
{
    // Boots the API in-process for integration tests (LocalDB: TechMovesLogisticsDB_Test).
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        private bool _databaseInitialized;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }

        public override HttpClient CreateClient(WebApplicationFactoryClientOptions options)
        {
            EnsureDatabase();
            return base.CreateClient(options);
        }

        public override HttpClient CreateClient()
        {
            EnsureDatabase();
            return base.CreateClient();
        }

        // Creates a fresh LocalDB schema for each test run.
        public void EnsureDatabase()
        {
            if (_databaseInitialized)
                return;

            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            _databaseInitialized = true;
        }
    }
}
