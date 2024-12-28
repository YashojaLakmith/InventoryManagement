using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Users.ListUsers;

public class ListUserRequestHandler : IRequestHandler<ListUserQuery, Result<ListUserQueryResult>>
{
    private readonly IValidator<ListUserQuery> _validator;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ListUserRequestHandler> _logger;

    public ListUserRequestHandler(
        IValidator<ListUserQuery> validator,
        ApplicationDbContext dbContext,
        ILogger<ListUserRequestHandler> logger)
    {
        _validator = validator;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<ListUserQueryResult>> Handle(ListUserQuery request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError<ListUserQueryResult>(validationResult.Errors);
        }

        List<UserListItem> users = await GetMatchingUsersAsync(request, cancellationToken);
        return new ListUserQueryResult(users, request.PageNumber, users.Count);
    }

    private Task<List<UserListItem>> GetMatchingUsersAsync(ListUserQuery request, CancellationToken cancellationToken)
    {
        int limit = request.PageSize;
        int offset = (request.PageNumber - 1) * request.PageSize;

        return _dbContext.Database
            .SqlQuery<UserListItem>(GetQueryString(request.Roles, limit, offset))
            .ToListAsync(cancellationToken);
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