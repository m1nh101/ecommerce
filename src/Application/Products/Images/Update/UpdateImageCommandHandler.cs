using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Images.Update;

public sealed class UpdateImageCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<UpdateImageCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateImageCommand command, CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var product = await dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        var imageId = new ProductImageId(command.ImageId);

        if (command.IsPrimary)
        {
            var result = product.SetPrimaryImage(imageId);
            if (result.IsFailure)
                return result.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
