using FluentValidation;

namespace InventoryManagement.Api.Features.InventoryItems.ListItems;

public class ListItemQueryValidator : AbstractValidator<ListItemsQuery>
{
    public ListItemQueryValidator()
    {
        RuleFor(query => query.ItemNamePartToSearch)
            .MaximumLength(50)
            .WithMessage("Maximum length of a item name is 50 characters.")
            .When(query => query.ItemNamePartToSearch != string.Empty && string.IsNullOrWhiteSpace(query.ItemNamePartToSearch));

        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number cannot be zero or a negative value.")
            .LessThan(int.MaxValue)
            .WithMessage("Page number is too high.");

        RuleFor(query => query.RecordsPerPage)
            .GreaterThanOrEqualTo(10)
            .WithMessage("Records per page must be greater than or equal to 10.")
            .LessThanOrEqualTo(100)
            .WithMessage("Records per page must be less than or equal to 100");
    }
}
