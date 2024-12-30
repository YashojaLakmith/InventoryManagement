using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using InventoryManagement.Api.Errors;

using MediatR;

namespace InventoryManagement.Api.Features.Users.ListUsers;

public class ListUserRequestHandler : IRequestHandler<ListUserQuery, Result<ListUserQueryResult>>
{
    private readonly IValidator<ListUserQuery> _validator;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ListUserRequestHandler> _logger;

    public ListUserRequestHandler(
        IValidator<ListUserQuery> validator,
        IUserRepository userRepository,
        ILogger<ListUserRequestHandler> logger)
    {
        _validator = validator;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<ListUserQueryResult>> Handle(ListUserQuery request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return InvalidDataError.CreateFailureResultFromError<ListUserQueryResult>(validationResult.Errors);
        }

        List<UserListItem> users = await _userRepository.ListUsersByFilterAsync(request, cancellationToken);
        return new ListUserQueryResult(users, request.PageNumber, users.Count);
    }
}