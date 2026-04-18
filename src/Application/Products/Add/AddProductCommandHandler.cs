using Application.Abstractions;
using Domain.Enums;
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
        if (!Enum.TryParse<Gender>(command.Gender, ignoreCase: true, out var gender))
            return ProductErrors.InvalidGender;

        var basePrice = new Money(command.BasePrice, command.Currency);
        var categoryId = new CategoryId(command.CategoryId);

        var result = Product.Create(command.Name, command.Description, command.Brand, categoryId, gender, basePrice);
        if (result.IsFailure)
            return result.Error;

        dbContext.Products.Add(result.Value);
        await dbContext.SaveChangesAsync(cancellationToken);

        return result.Value.Id.Value;
    }
}
