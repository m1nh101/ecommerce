using Domain.Orders;
using Domain.Primitives;

namespace Domain.Payments.Events;

public sealed record PaymentFailedDomainEvent(PaymentId PaymentId, OrderId OrderId, string Reason) : IDomainEvent;
