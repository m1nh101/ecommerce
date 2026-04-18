using Domain.Enums;
using FluentValidation;

namespace Application.Products.Update;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Brand)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Gender)
            .NotEmpty()
            .Must(g => Enum.TryParse<Gender>(g, ignoreCase: true, out _))
            .WithMessage("Gender must be one of: Men, Women, Unisex, Kids.");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(3);
    }
}
