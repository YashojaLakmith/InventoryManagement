using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

namespace InventoryManagement.Api.Features.Users.ViewUser;

public class ViewUserQueryHandler : IRequestHandler<UserIdQuery, Result<UserView>>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UserIdQuery> _validator;
    private readonly ILogger<ViewUserQueryHandler> _logger;

    public ViewUserQueryHandler(
        IUserRepository userRepository,
        IValidator<UserIdQuery> validator,
        ILogger<ViewUserQueryHandler> logger)
    {
        _userRepository = userRepository;
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

        UserView? user = await _userRepository.GetUserWithRolesByIdAsync(request.UserId, cancellationToken);

        return user is null
            ? NotFoundError.CreateFailureResultFromError<UserView>($"User with Id: {request.UserId}")
            : user;
    }
}