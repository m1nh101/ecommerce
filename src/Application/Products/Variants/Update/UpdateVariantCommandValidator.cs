using FluentValidation;

namespace Application.Products.Variants.Update;

public sealed class UpdateVariantCommandValidator : AbstractValidator<UpdateVariantCommand>
{
    public UpdateVariantCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.VariantId).NotEmpty();
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PriceOverride)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PriceOverride.HasValue);
        RuleFor(x => x.PriceOverrideCurrency)
            .NotEmpty().MaximumLength(3)
            .When(x => x.PriceOverride.HasValue);
    }
}
