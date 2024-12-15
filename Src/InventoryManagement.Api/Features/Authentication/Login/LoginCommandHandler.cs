using FluentResults;

using FluentValidation;
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
    private readonly IValidator<LoginInformation> _validator;

    public LoginCommandHandler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILogger<LoginCommandHandler> logger,
        IValidator<LoginInformation> validator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result> Handle(LoginInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = _validator.Validate(request);
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
