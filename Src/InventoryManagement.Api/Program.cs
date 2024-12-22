using System.Reflection;

using FluentValidation;

using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Infrastructure.Database;
using InventoryManagement.Api.Infrastructure.Email;
using InventoryManagement.Api.Utilities;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api;

public class Program
{
    public static async Task Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        Assembly assembly = typeof(Program).Assembly;

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<ApplicationDbContext>();
        builder.Services.AddMediatR(o => o.RegisterServicesFromAssembly(assembly));
        builder.Services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Singleton);
        builder.Services.AddSingleton<IEmailSender<User>, EmailSender>();
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
            config.Password.RequiredLength = 7;
            config.Password.RequireNonAlphanumeric = false;
            config.Password.RequireLowercase = true;
            config.Password.RequireUppercase = true;
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