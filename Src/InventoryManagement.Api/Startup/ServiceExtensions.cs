using System.Reflection;
using System.Security.Claims;

using FluentValidation;

using InventoryManagement.Api.Features.Shared.Abstractions;
using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Infrastructure.Caching;
using InventoryManagement.Api.Infrastructure.Database;
using InventoryManagement.Api.Infrastructure.Database.Repositories;
using InventoryManagement.Api.Infrastructure.Email;
using InventoryManagement.Api.Infrastructure.Reports;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Startup;

public static class ServiceExtensions
{
    private static readonly Assembly Assembly = typeof(ServiceExtensions).Assembly;

    public static void AddSingletonServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly, ServiceLifetime.Singleton);
        services.AddSingleton<IEmailSender<User>, EmailSender>();
        services.AddSingleton<ITimeProvider, Features.Shared.TimeProvider>();
    }

    public static void AddScopedServices(this WebApplicationBuilder builder)
    {
        ConfigureClaimsPrincipalInjection(builder.Services);
        builder.Services.AddRepositoryImplementations();
        builder.Services.AddReportGenerators();
        builder.AddNpgsqlDbContext<ApplicationDbContext>("inventory-management-db");
    }

    public static void AddTransientServices(this IServiceCollection services)
    {
        services.AddMediatR(o => o.RegisterServicesFromAssembly(Assembly));
    }

    /// <summary>
    /// Call this after injecting caching services.
    /// </summary>
    public static void ConfigureAuthentication(this IServiceCollection services)
    {
        services.AddSingleton<ITicketStore, TicketStore>();
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

    public static void ConfigureCaching(this WebApplicationBuilder builder)
    {
        builder.AddRedisDistributedCache("redis-cache");
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        builder.Services.AddHybridCache();
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
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
