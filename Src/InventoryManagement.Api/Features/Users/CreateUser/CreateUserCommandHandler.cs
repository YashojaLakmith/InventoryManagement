﻿using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Api.Features.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<NewUserInformation, Result<int>>
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

    public async Task<Result<int>> Handle(NewUserInformation request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError(validationResult.Errors);
        }

        bool isEmailInUse = await _userManager.Users
            .AnyAsync(user => 
                user.Email!.Equals(request.EmailAddress, StringComparison.OrdinalIgnoreCase), cancellationToken);
        if (isEmailInUse)
        {
            return AlreadyExistsError.CreateFailureResultFromError<int>($@"User with provided email address");
        }

        User newUser = User.Create(request.EmailAddress, request.UserName);
        IdentityResult identityResult = await _userManager.CreateAsync(newUser, request.Password);

        return !identityResult.Succeeded
            ? AlreadyExistsError.CreateFailureResultFromError<int>($@"User with provided email address")
            : Result.Ok(newUser.Id);
    }
}
