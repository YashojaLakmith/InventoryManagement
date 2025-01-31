using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Infrastructure.Database;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Startup;

public class InitialDatabaseSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ILogger<InitialDatabaseSeeder> _logger;

    public InitialDatabaseSeeder(
        ApplicationDbContext dbContext,
        UserManager<User> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        ILogger<InitialDatabaseSeeder> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync()
    {
        try
        {
            await SeedRolesAsync();
            await CreateSuperUser();
        }
        catch (Exception e)
        {
            _logger.LogCritical(@"Database seeding error. {error}", e);
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        if (_dbContext.Roles.Any())
        {
            return;
        }

        List<IdentityRole<int>> roles = [
            new(Roles.SuperUser),
            new(Roles.UserManager),
            new(Roles.Issuer),
            new(Roles.Receiver),
            new(Roles.ScheduleManager)
            ];

        foreach (IdentityRole<int> role in roles)
        {
            await _roleManager.CreateAsync(role);
        }

    }

    private async Task CreateSuperUser()
    {
        if (_dbContext.Users.Any())
        {
            return;
        }

        const string password = @"Superuser123";
        const string email = @"system@domain.io";
        const string name = @"System";
        User superUser = User.Create(email, name);

        await _userManager.CreateAsync(superUser, password);
        await AssignSuperUserRoleAsync(email);
    }

    private async Task AssignSuperUserRoleAsync(string email)
    {
        User? superUser = await _userManager.FindByEmailAsync(email);
        IdentityRole<int>? role = await _roleManager.FindByNameAsync(Roles.SuperUser);
        await _userManager.AddToRoleAsync(superUser!, role!.Name!);
    }
}
