using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Variants.Remove;

public sealed class RemoveVariantCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<RemoveVariantCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveVariantCommand command, CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var variantId = new ProductVariantId(command.VariantId);

        var variant = await dbContext.ProductVariants
            .FirstOrDefaultAsync(v => v.Id == variantId && v.ProductId == productId, cancellationToken);

        if (variant is null)
            return ProductErrors.VariantNotFound;

        dbContext.ProductVariants.Remove(variant);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
