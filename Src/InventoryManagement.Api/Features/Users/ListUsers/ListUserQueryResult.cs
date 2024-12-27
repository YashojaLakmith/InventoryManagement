namespace InventoryManagement.Api.Features.Users.ListUsers;

public record ListUserQueryResult(IReadOnlyCollection<UserListItem> Users, int Page, int TotalCount);