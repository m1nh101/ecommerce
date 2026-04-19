using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Images.Add;

public sealed class AddImageCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<AddImageCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(AddImageCommand command, CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var product = await dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        ProductVariantId? variantId = command.VariantId.HasValue
            ? new ProductVariantId(command.VariantId.Value)
            : null;

        var result = product.AddImage(command.Url, variantId, command.SortOrder);
        if (result.IsFailure)
            return result.Error;

        if (command.IsPrimary)
        {
            var primaryResult = product.SetPrimaryImage(result.Value.Id);
            if (primaryResult.IsFailure)
                return primaryResult.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id.Value;
    }
}
