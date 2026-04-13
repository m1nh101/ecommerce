using Domain.Orders;
using Domain.Primitives;

namespace Domain.Payments.Events;

public sealed record PaymentCompletedDomainEvent(PaymentId PaymentId, OrderId OrderId) : IDomainEvent;
