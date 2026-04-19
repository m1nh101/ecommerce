using Domain.Primitives;
using Mediator;

namespace Application.Products.Variants.Add;

public sealed record AddVariantCommand(
    Guid ProductId,
    Guid ColorId,
    Guid SizeId,
    string Sku,
    decimal? PriceOverride,
    string? PriceOverrideCurrency,
    int StockQuantity) : ICommand<Result<Guid>>;
