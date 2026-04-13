using Domain.Primitives;
using Domain.ValueObjects;

namespace Domain.Products.Events;

public sealed record ProductPriceChangedDomainEvent(ProductId ProductId, Money OldPrice, Money NewPrice) : IDomainEvent;
