using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Delete;

public sealed class DeleteProductCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<DeleteProductCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var product = await dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return ProductErrors.NotFound;

        product.Deactivate();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
