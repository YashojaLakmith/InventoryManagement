using InventoryManagement.Api.Startup;
using InventoryManagement.Api.Utilities;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api;

public class Program
{
    public static async Task Main()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.ConfigureCaching();
        builder.Services.ConfigureAuthentication();
        builder.AddServiceDefaults();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();
        builder.Services.AddSingletonServices();
        builder.AddScopedServices();
        builder.Services.AddTransientServices();

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
}