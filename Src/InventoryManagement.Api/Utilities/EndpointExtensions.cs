namespace InventoryManagement.Api.Utilities;

public static class EndpointExtensions
{
    public static void AddApiEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        IEnumerable<IEndpoint> endpointDeclarations = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(type => type.IsAssignableTo(typeof(IEndpoint)) && type.IsClass)
            .Select(type => Activator.CreateInstance(type)!)
            .OfType<IEndpoint>();

        foreach (IEndpoint endpoint in endpointDeclarations)
        {
            endpoint.MapEndpoint(routeBuilder);
        }
    }
}
