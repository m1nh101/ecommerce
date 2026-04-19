using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Domain.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Variants.Add;

public sealed class AddVariantCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<AddVariantCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(AddVariantCommand command, CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var product = await dbContext.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        var colorId = new ColorId(command.ColorId);
        var sizeId = new SizeId(command.SizeId);

        Money? priceOverride = command.PriceOverride.HasValue
            ? new Money(command.PriceOverride.Value, command.PriceOverrideCurrency ?? "USD")
            : null;

        var result = product.AddVariant(colorId, sizeId, command.Sku, priceOverride, command.StockQuantity);
        if (result.IsFailure)
            return result.Error;

        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id.Value;
    }
}
