using FluentResults;

using FluentValidation.Results;

using InventoryManagement.Api.Features.Authentication.Errors;
using InventoryManagement.Api.Features.Users;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Api.Features.Authentication.Login;

public class LoginCommandHandler : IRequestHandler<LoginInformation, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<Result> Handle(LoginInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        LoginInformationValidator validator = new();
        ValidationResult validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            IEnumerable<string> errors = validationResult.Errors
                .Select(err => err.ErrorMessage);
            return Result.Fail(new InvalidLoginInformationError(errors));
        }

        User? user = await _userManager.FindByEmailAsync(request.EmailAddress);
        if (user is null)
        {
            return Result.Fail(new UserNotFoundError());
        }

        SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);
        return !signInResult.Succeeded
            ? Result.Fail(new IncorrectPasswordError())
            : Result.Ok();
    }
}
