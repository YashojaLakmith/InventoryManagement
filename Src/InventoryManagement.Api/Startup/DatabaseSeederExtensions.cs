using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Infrastructure.Database;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Startup;

public static class DatabaseSeederExtensions
{
    public static async Task UseDatabaseMigrationsAndSeeding(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

        ApplicationDbContext dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        UserManager<User> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
        RoleManager<IdentityRole<int>> roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        ILogger<InitialDatabaseSeeder> logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<InitialDatabaseSeeder>>();

        await dbContext.Database.MigrateAsync();
        await dbContext.Database.EnsureCreatedAsync();

        InitialDatabaseSeeder seeder = new(dbContext, userManager, roleManager, logger);
        await seeder.SeedDatabaseAsync();
    }
}
