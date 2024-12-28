namespace InventoryManagement.Api.Features.Users;

public static class BatchEndpointNameConstants
{
    private const string GroupName = @"Users";
    public const string LoginEndpoint = @"Login";
    public const string LogoutEndpoint = @"Logout";
    public const string RequestResetPassword = @"Request Password Reset";
    public const string ResetPassword = @"Reset Password";

    public static RouteHandlerBuilder WithUserEndpointName(this RouteHandlerBuilder builder, string endpointName)
    {
        return builder
            .WithGroupName(GroupName)
            .WithName(endpointName);
    }
}
