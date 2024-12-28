using System.Reflection;
using System.Security.Claims;

using FluentValidation;

using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Infrastructure.Database;
using InventoryManagement.Api.Infrastructure.Email;
using InventoryManagement.Api.Startup;
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
        ConfigureAuthentication(builder.Services);
        ConfigureClaimsPrincipalInjection(builder.Services);
        builder.Services.AddDbContext<ApplicationDbContext>();
        builder.Services.AddMediatR(o => o.RegisterServicesFromAssembly(assembly));
        builder.Services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Singleton);
        builder.Services.AddSingleton<IEmailSender<User>, EmailSender>();

        WebApplication app = builder.Build();

        if (builder.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapSwagger();
        app.AddApiEndpoints();
        await app.UseDatabaseMigrationsAndSeeding();
        await app.RunAsync();
    }

    private static void ConfigureAuthentication(IServiceCollection services)
    {
        services.AddIdentityCore<User>(config =>
            {
                config.Password.RequiredLength = 7;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireLowercase = true;
                config.Password.RequireUppercase = true;

                config.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<int>>()
            .AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.ApplicationScheme, config =>
            {
                config.Cookie.HttpOnly = true;
                config.Cookie.IsEssential = true;
                config.SlidingExpiration = true;
                config.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                config.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                config.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        services.AddAuthorization();
    }

    private static void ConfigureClaimsPrincipalInjection(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ClaimsPrincipal>(serviceProvider =>
        {
            IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            return httpContextAccessor.HttpContext!.User;
        });
    }
}