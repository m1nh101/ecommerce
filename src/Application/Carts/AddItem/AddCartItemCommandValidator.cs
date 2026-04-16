using FluentValidation;

namespace Application.Carts.AddItem;

public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
