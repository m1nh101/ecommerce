using Domain.Orders;
using Domain.Primitives;

namespace Domain.Orders.Events;

public sealed record OrderCancelledDomainEvent(OrderId OrderId) : IDomainEvent;
