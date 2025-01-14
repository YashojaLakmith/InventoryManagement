using InventoryManagement.Api.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.IntegrationTests.RepositoryTests;

[TestFixture]
public abstract class BaseRepositoryIntegrationTest
{
    private TestApplicationFactory _applicationFactory;
    private IServiceScope _serviceScope;
    private ApplicationDbContext _dbContext;

    protected ApplicationDbContext DbContext => _dbContext;

    [SetUp]
    public async Task SetupAsync()
    {
        _applicationFactory = await TestApplicationFactory.BuildTestApplicationFactoryAsync();
        _serviceScope = _applicationFactory.Services.CreateScope();
        _dbContext = _serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await _dbContext.Database.MigrateAsync();
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await _dbContext.DisposeAsync();
        _serviceScope.Dispose();
        await _applicationFactory.DisposeAsync();
    }
}
