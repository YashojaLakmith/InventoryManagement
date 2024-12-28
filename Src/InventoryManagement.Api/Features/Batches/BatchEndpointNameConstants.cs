namespace InventoryManagement.Api.Features.Batches;

public static class BatchEndpointNameConstants
{
    private const string GroupName = @"Batches";
    public const string CreateBatch = @"Create Batch";
    public const string DeleteBatchLine = @"Delete Batch Line";

    public static RouteHandlerBuilder WithBatchEndpointName(this RouteHandlerBuilder builder, string endpointName)
    {
        return builder
            .WithGroupName(GroupName)
            .WithName(endpointName);
    }
}
