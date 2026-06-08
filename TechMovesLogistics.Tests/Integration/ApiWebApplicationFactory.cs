using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechMovesLogistics.Api.Controllers;
using TechMoves_Logistics.Data;

namespace TechMovesLogistics.Tests.Integration
{
    // Boots the API in-process for integration tests (LocalDB: TechMovesLogisticsDB_Test).
    public class ApiWebApplicationFactory : WebApplicationFactory<ContractsController>
    {
        private bool _databaseInitialized;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            if (!_databaseInitialized)
            {
                using var scope = host.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                _databaseInitialized = true;
            }

            return host;
        }
    }
}
