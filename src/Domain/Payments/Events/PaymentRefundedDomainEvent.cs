using Domain.Orders;
using Domain.Primitives;

namespace Domain.Payments.Events;

public sealed record PaymentRefundedDomainEvent(PaymentId PaymentId, OrderId OrderId) : IDomainEvent;
