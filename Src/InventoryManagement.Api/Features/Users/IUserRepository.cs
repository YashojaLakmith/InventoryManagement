using InventoryManagement.Api.Features.Users.ListUsers;
using InventoryManagement.Api.Features.Users.ViewUser;

namespace InventoryManagement.Api.Features.Users;

public interface IUserRepository
{
    Task<HashSet<string>> GetAllRoleNamesAsync(CancellationToken cancellationToken = default);
    Task<List<UserListItem>> ListUsersByFilterAsync(ListUserQuery query, CancellationToken cancellationToken = default);
    Task<UserView?> GetUserWithRolesByIdAsync(int userId, CancellationToken cancellationToken = default);
}
