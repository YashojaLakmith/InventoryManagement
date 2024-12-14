using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Infrastructure.Database;
using InventoryManagement.Api.Utilities;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api;

public class Program
{
    public static async Task Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<ApplicationDbContext>();
        builder.Services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));
        ConfigureIdentity(builder.Services);

        WebApplication app = builder.Build();

        if (builder.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapSwagger().RequireAuthorization();
        app.AddApiEndpoints();
        await app.RunAsync();
    }

    private static void ConfigureIdentity(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        });

        services.AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(config =>
        {
            config.Password.RequiredLength = 6;
            config.User.RequireUniqueEmail = true;
        });

        services.ConfigureApplicationCookie(config =>
        {
            config.Cookie.HttpOnly = true;
            config.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            config.SlidingExpiration = true;
        });
    }
}