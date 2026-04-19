using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Domain.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Variants.Update;

public sealed class UpdateVariantCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<UpdateVariantCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateVariantCommand command, CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var product = await dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        var variantId = new ProductVariantId(command.VariantId);
        var variant = product.Variants.FirstOrDefault(v => v.Id == variantId);
        if (variant is null)
            return ProductErrors.VariantNotFound;

        Money? priceOverride = command.PriceOverride.HasValue
            ? new Money(command.PriceOverride.Value, command.PriceOverrideCurrency ?? "USD")
            : null;

        var priceResult = product.SetVariantPriceOverride(variantId, priceOverride);
        if (priceResult.IsFailure)
            return priceResult.Error;

        var delta = command.StockQuantity - variant.StockQuantity;
        var stockResult = product.AdjustVariantStock(variantId, delta);
        if (stockResult.IsFailure)
            return stockResult.Error;

        var activationResult = command.IsActive
            ? product.ActivateVariant(variantId)
            : product.DeactivateVariant(variantId);

        if (activationResult.IsFailure)
            return activationResult.Error;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
