using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.ListUsers;

public record ListUserQuery(
    int PageSize,
    int PageNumber,
    IReadOnlyCollection<string> Roles) : IRequest<Result<ListUserQueryResult>>;