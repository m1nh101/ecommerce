using Domain.Primitives;
using Mediator;

namespace Application.Products.Variants.Update;

public sealed record UpdateVariantCommand(
    Guid ProductId,
    Guid VariantId,
    decimal? PriceOverride,
    string? PriceOverrideCurrency,
    int StockQuantity,
    bool IsActive) : ICommand<Result>;
