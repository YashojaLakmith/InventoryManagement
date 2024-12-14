using FluentResults;

using MediatR;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<NewUserInformation, Result>
{
    public Task<Result> Handle(NewUserInformation request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
