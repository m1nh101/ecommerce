using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Images.Remove;

public sealed class RemoveImageCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<RemoveImageCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveImageCommand command, CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var product = await dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        var imageId = new ProductImageId(command.ImageId);
        var result = product.RemoveImage(imageId);
        if (result.IsFailure)
            return result.Error;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
