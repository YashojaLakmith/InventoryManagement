using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<NewUserInformation, Result>
{
    private readonly IValidator<NewUserInformation> _validator;
    private readonly ILogger<CreateUserCommandHandler> _logger;
    private readonly UserManager<User> _userManager;

    public CreateUserCommandHandler(
        IValidator<NewUserInformation> validator,
        ILogger<CreateUserCommandHandler> logger,
        UserManager<User> userManager)
    {
        _validator = validator;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Result> Handle(NewUserInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            throw new NotImplementedException();
        }

        User? existingUser = await _userManager.FindByEmailAsync(request.EmailAddress);
        if (existingUser is not null)
        {
            throw new NotImplementedException();
        }

        User newUser = User.Create(request.EmailAddress, request.UserName);
        await _userManager.CreateAsync(newUser, request.Password);

        return Result.Ok();
    }
}
