using Application.Abstractions;
using Domain.Enums;
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
            return ProductErrors.NotFound;

        if (!Enum.TryParse<Gender>(command.Gender, ignoreCase: true, out var gender))
            return ProductErrors.InvalidGender;

        product.UpdateDetails(command.Name, command.Description, command.Brand, new CategoryId(command.CategoryId), gender);

        var priceResult = product.UpdateBasePrice(new Money(command.BasePrice, command.Currency));
        if (priceResult.IsFailure)
            return priceResult;

        if (command.IsActive)
            product.Activate();
        else
            product.Deactivate();

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
