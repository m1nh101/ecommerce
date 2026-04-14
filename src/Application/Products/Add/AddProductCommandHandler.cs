using Application.Abstractions;
using Domain.Primitives;
using Domain.Products;
using Domain.ValueObjects;
using Mediator;

namespace Application.Products.Add;

public sealed class AddProductCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<AddProductCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(AddProductCommand command, CancellationToken cancellationToken)
    {
        var price = new Money(command.Price, command.Currency);

        var productCreationResult = Product.Create(
            command.Name,
            command.Description,
            price,
            command.StockQuantity,
            command.Category);

        if (productCreationResult.IsFailure)
            return productCreationResult.Error;

        dbContext.Products.Add(productCreationResult.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return productCreationResult.Value.Id.Value;
    }
}
