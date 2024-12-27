namespace InventoryManagement.Api.Features.Users.ViewUser;

public record UserView(
    int UserId,
    string UserName,
    string EmailAddress,
    IReadOnlyCollection<string> Roles);