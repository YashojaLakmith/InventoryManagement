internal class Program
{
    private static void Main(string[] args)
    {
        IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

        IResourceBuilder<RedisResource> redisCache = builder.AddRedis("Redis")
            .WithImage("redis", "latest");

        IResourceBuilder<PostgresServerResource> postgresInstance = builder.AddPostgres("Postgres")
            .WithImage("postgres", "16")
            .WithDataVolume("pg-inventoryManagement-data");
        IResourceBuilder<PostgresDatabaseResource> postgresDb = postgresInstance.AddDatabase("InventoryManagementDb");

        builder.AddProject<Projects.InventoryManagement_Api>("InventoryManagementApi", "http")
            .WithReference(redisCache)
            .WithReference(postgresDb)
            .WaitFor(redisCache)
            .WaitFor(postgresDb);

        builder.Build().Run();
    }
}