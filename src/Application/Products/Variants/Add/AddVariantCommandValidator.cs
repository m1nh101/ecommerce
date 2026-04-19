using FluentValidation;

namespace Application.Products.Variants.Add;

public sealed class AddVariantCommandValidator : AbstractValidator<AddVariantCommand>
{
    public AddVariantCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.ColorId).NotEmpty();
        RuleFor(x => x.SizeId).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PriceOverride)
            .GreaterThanOrEqualTo(0)
            .When(x => x.PriceOverride.HasValue);
        RuleFor(x => x.PriceOverrideCurrency)
            .NotEmpty().MaximumLength(3)
            .When(x => x.PriceOverride.HasValue);
    }
}
