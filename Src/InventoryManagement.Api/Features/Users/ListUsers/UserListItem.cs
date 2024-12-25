namespace InventoryManagement.Api.Features.Users.ListUsers;

public record UserListItem(
    int UserId,
    string UserName,
    string EmailAddress);