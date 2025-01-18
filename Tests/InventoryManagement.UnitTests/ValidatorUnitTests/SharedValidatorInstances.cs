using InventoryManagement.Api.Features.Shared.Validators;

namespace InventoryManagement.UnitTests.ValidatorUnitTests;
internal static class SharedValidatorInstances
{
    public static readonly BatchNumberValidator BatchNumberValidator = new();
    public static readonly EmailValidator EmailValidator = new();
    public static readonly PasswordValidator PasswordValidator = new();
    public static readonly InventoryItemNumberValidator InventoryItemNumberValidator = new();
    public static readonly RoleNameValidator RoleNameValidator = new();
    public static readonly UserIdValidator UserIdValidator = new();
}
