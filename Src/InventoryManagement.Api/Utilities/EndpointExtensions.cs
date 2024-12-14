namespace InventoryManagement.Api.Utilities;

public static class EndpointExtensions
{
    public static void AddApiEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        IEnumerable<IEndpoint> endpointDeclarations = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(type => type.IsAssignableTo(typeof(IEndpoint)) && type.IsClass)
            .Select(type => (IEndpoint)Activator.CreateInstance(type)!);

        foreach (IEndpoint endpoint in endpointDeclarations)
        {
            endpoint.MapEndpoint(routeBuilder);
        }
    }
}
