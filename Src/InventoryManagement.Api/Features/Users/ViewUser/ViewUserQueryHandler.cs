using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Api.Errors;
using InventoryManagement.Api.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
        
        Tuple<int, string, string>? userDetails = await GetUserDetailsAsync(request, cancellationToken);
        if (userDetails is null)
        {
            return NotFoundError.CreateFailureResultFromError<UserView>($"User with Id: {request.UserId}");
        }
        
        List<string> roles = await GetUserRolesAsync(request, cancellationToken);

        return new UserView(userDetails.Item1, userDetails.Item2, userDetails.Item3, roles);
    }

    private Task<Tuple<int, string, string>?> GetUserDetailsAsync(UserIdQuery request, CancellationToken cancellationToken)
    {
        return _dbContext.Users
            .AsNoTracking()
            .Select(user => Tuple.Create(user.Id, user.UserName!, user.Email!))
            .SingleOrDefaultAsync(user => user.Item1 == request.UserId, cancellationToken);
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