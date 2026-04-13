using Domain.Primitives;

namespace Domain.Orders.Events;

public sealed record OrderShippedDomainEvent(OrderId OrderId) : IDomainEvent;
