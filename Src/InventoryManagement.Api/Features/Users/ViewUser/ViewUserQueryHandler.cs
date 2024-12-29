using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Infrastructure.Database;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Users.ViewUser;

public class ViewUserQueryHandler : IRequestHandler<UserIdQuery, Result<UserView>>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IValidator<UserIdQuery> _validator;
    private readonly ILogger<ViewUserQueryHandler> _logger;

    public ViewUserQueryHandler(
        ApplicationDbContext dbContext,
        IValidator<UserIdQuery> validator,
        ILogger<ViewUserQueryHandler> logger)
    {
        _dbContext = dbContext;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<UserView>> Handle(UserIdQuery request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError<UserView>(validationResult.Errors);
        }

        var userDetails = await _dbContext.Users
            .AsNoTracking()
            .Select(user => new { user.Id, user.UserName, user.Email })
            .SingleOrDefaultAsync(user => user.Id == request.UserId, cancellationToken);

        if (userDetails is null)
        {
            return NotFoundError.CreateFailureResultFromError<UserView>($"User with Id: {request.UserId}");
        }

        List<string> roles = await GetUserRolesAsync(request, cancellationToken);

        return new UserView(userDetails.Id, userDetails.UserName!, userDetails.Email!, roles);
    }

    private Task<List<string>> GetUserRolesAsync(UserIdQuery request, CancellationToken cancellationToken)
    {
        return _dbContext.Roles
            .FromSqlInterpolated(GetInterpolatedQuery(request.UserId))
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
}