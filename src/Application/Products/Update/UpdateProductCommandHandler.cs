using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Domain.ValueObjects;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Update;

public sealed class UpdateProductCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<UpdateProductCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var product = await dbContext.Products
            .FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        product.UpdateDetails(command.Name, command.Description, command.Category);

        var priceResult = product.UpdatePrice(new Money(command.Price, command.Currency));
        if (priceResult.IsFailure)
            return priceResult;

        var delta = command.StockQuantity - product.StockQuantity;
        if (delta > 0)
        {
            var r = product.IncrementStock(delta);
            if (r.IsFailure) return r;
        }
        else if (delta < 0)
        {
            var r = product.DecrementStock(-delta);
            if (r.IsFailure) return r;
        }

        if (command.IsActive)
            product.Activate();
        else
            product.Deactivate();

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
