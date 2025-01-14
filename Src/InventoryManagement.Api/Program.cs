using System.Reflection;
using System.Security.Claims;

using FluentValidation;

using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Infrastructure.Caching;
using InventoryManagement.Api.Infrastructure.Database;
using InventoryManagement.Api.Infrastructure.Database.Repositories;
using InventoryManagement.Api.Infrastructure.Email;
using InventoryManagement.Api.Infrastructure.Reports;
using InventoryManagement.Api.Startup;
using InventoryManagement.Api.Utilities;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api;

public class Program
{
    public static async Task Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        Assembly assembly = typeof(Program).Assembly;

        ConfigureCaching(builder.Services);
        builder.Services.AddSingleton<ITicketStore, TicketStore>();
        ConfigureAuthentication(builder.Services);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();
        ConfigureClaimsPrincipalInjection(builder.Services);
        builder.Services.AddDbContext<ApplicationDbContext>();
        builder.Services.AddRepositoryImplementations();
        builder.Services.AddReportGenerators();
        builder.Services.AddMediatR(o => o.RegisterServicesFromAssembly(assembly));
        builder.Services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Singleton);
        builder.Services.AddSingleton<IEmailSender<User>, EmailSender>();

        WebApplication app = builder.Build();

        if (builder.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseDeveloperExceptionPage();
        }

        app.UseAuthentication();
        app.UseAuthorization();
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
                config.SessionStore = services.BuildServiceProvider().GetRequiredService<ITicketStore>();

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

    private static void ConfigureCaching(IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }
}