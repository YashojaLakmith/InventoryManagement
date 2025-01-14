using InventoryManagement.Api;
using InventoryManagement.Api.Infrastructure.Database;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

namespace InventoryManagement.IntegrationTests;
public sealed class TestApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _dbContainer;

    public static async Task<TestApplicationFactory> BuildTestApplicationFactoryAsync()
    {
        TestApplicationFactory factory = new();
        await factory._dbContainer.StartAsync();
        return factory;
    }

    private TestApplicationFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
        .WithImage(@"postgres:16")
        .WithUsername(@"postgres")
        .WithPassword(@"postgres")
        .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            Type descriptorType = typeof(DbContextOptions<ApplicationDbContext>);
            ServiceDescriptor? descriptor = services.SingleOrDefault(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await base.DisposeAsync();
    }
}
