using Domain.Orders;
using Domain.Primitives;

namespace Domain.Orders.Events;

public sealed record OrderPlacedDomainEvent(OrderId OrderId) : IDomainEvent;
