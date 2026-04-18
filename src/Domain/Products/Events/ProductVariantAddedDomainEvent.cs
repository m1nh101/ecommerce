using Domain.Primitives;

namespace Domain.Products.Events;

public sealed record ProductVariantAddedDomainEvent(
    ProductId ProductId,
    ProductVariantId VariantId,
    string Sku) : IDomainEvent;
