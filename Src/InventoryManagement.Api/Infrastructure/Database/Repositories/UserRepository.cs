using InventoryManagement.Api.Features.Users;
using InventoryManagement.Api.Features.Users.ListUsers;
using InventoryManagement.Api.Features.Users.ViewUser;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Infrastructure.Database.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<HashSet<string>> GetAllRoleNamesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles
            .AsNoTracking()
            .Select(role => role.Name!)
            .ToHashSetAsync(cancellationToken);
    }

    public async Task<UserView?> GetUserWithRolesByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userDetails = await _dbContext.Users
            .AsNoTracking()
            .Select(user => new { user.Id, user.UserName, user.Email })
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

        if (userDetails is null)
        {
            return null;
        }

        List<string> roles = await GetUserRolesAsync(userId, cancellationToken);

        return new UserView(userDetails.Id, userDetails.UserName!, userDetails.Email!, roles);
    }

    public Task<List<UserListItem>> ListUsersByFilterAsync(ListUserQuery query, CancellationToken cancellationToken = default)
    {
        int limit = query.PageSize;
        int offset = (query.PageSize - 1) * query.PageNumber;

        return _dbContext.Database
            .SqlQuery<UserListItem>(GetQueryString(query.Roles, limit, offset))
            .ToListAsync(cancellationToken);
    }

    private Task<List<string>> GetUserRolesAsync(int userId, CancellationToken cancellationToken)
    {
        return _dbContext.Roles
            .FromSqlInterpolated(GetInterpolatedQuery(userId))
            .AsNoTracking()
            .Select(role => role.Name!)
            .ToListAsync(cancellationToken);
    }

    private static FormattableString GetInterpolatedQuery(int userId)
    {
        return $"""
                SELECT r."Id", r."Name", r."NormalizedName", r."ConcurrencyStamp"
                FROM "AspNetRoles" AS r
                INNER JOIN "AspNetUserRoles" AS ur ON r."Id" = ur."RoleId"
                WHERE ur."UserId" = {userId}
                ORDER BY r."Name"
                """;
    }

    private static FormattableString GetQueryString(IReadOnlyCollection<string> roles, int limit, int offset)
    {
        string roleList = string.Join(',', roles.Select(r => $@"{r.ToUpper()}"));

        return $"""
                SELECT u."Id" AS "UserId", u."UserName" AS "UserName", u."Email" AS "EmailAddress"
                FROM "AspNetUsers" AS u
                INNER JOIN "AspNetUserRoles" AS ur ON u."Id" = ur."UserId"
                INNER JOIN "AspNetRoles" AS r ON ur."RoleId" = r."Id"
                WHERE r."Name" IN ({roleList})
                ORDER BY u."Id"
                LIMIT {limit}
                OFFSET {offset}
                """;
    }
}
