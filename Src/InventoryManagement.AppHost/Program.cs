internal class Program
{
    private static void Main(string[] args)
    {
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

        IResourceBuilder<RedisResource> redisCache = builder.AddRedis("redis-cache")
            .WithImage("redis", "latest");

        IResourceBuilder<PostgresServerResource> postgresInstance = builder.AddPostgres("postgres-server")
            .WithImage("postgres", "16")
            .WithDataVolume("pg-inventoryManagement-data");
        IResourceBuilder<PostgresDatabaseResource> postgresDb = postgresInstance.AddDatabase("inventory-management-db");

        builder.AddProject<Projects.InventoryManagement_Api>("inventoryManagement-api", "http")
            .WithExternalHttpEndpoints()
            .WithReference(redisCache)
            .WithReference(postgresDb)
            .WaitFor(redisCache)
            .WaitFor(postgresDb);

        builder.Build().Run();
    }
}