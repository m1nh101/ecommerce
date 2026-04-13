using Domain.Enums;
using Domain.Orders;
using Domain.Primitives;
using Domain.ValueObjects;
using Domain.Payments.Events;

namespace Domain.Payments;

public sealed class Payment : AggregateRoot<PaymentId>
{
    private Payment() { }

    private Payment(PaymentId id, OrderId orderId, Money amount, PaymentMethod method, DateTime initiatedAt)
        : base(id)
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
        InitiatedAt = initiatedAt;
        CreatedAt = initiatedAt;
        UpdatedAt = initiatedAt;
    }

    public OrderId OrderId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string? ExternalTransactionId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime InitiatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Payment Initiate(OrderId orderId, Money amount, PaymentMethod method)
    {
        var now = DateTime.UtcNow;
        return new Payment(PaymentId.New(), orderId, amount, method, now);
    }

    public Result MarkProcessing()
    {
        if (Status != PaymentStatus.Pending)
            return Result.Failure(PaymentErrors.CannotProcess);

        Status = PaymentStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Complete(string externalTransactionId)
    {
        if (Status != PaymentStatus.Processing)
            return Result.Failure(PaymentErrors.CannotComplete);

        if (string.IsNullOrWhiteSpace(externalTransactionId))
            return Result.Failure(PaymentErrors.MissingTransactionId);

        Status = PaymentStatus.Completed;
        ExternalTransactionId = externalTransactionId;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentCompletedDomainEvent(Id, OrderId));
        return Result.Success();
    }

    public Result Fail(string reason)
    {
        if (Status != PaymentStatus.Processing)
            return Result.Failure(PaymentErrors.CannotFail);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(PaymentErrors.MissingFailureReason);

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentFailedDomainEvent(Id, OrderId, reason));
        return Result.Success();
    }

    public Result Refund()
    {
        if (Status != PaymentStatus.Completed)
            return Result.Failure(PaymentErrors.CannotRefund);

        Status = PaymentStatus.Refunded;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RaiseDomainEvent(new PaymentRefundedDomainEvent(Id, OrderId));
        return Result.Success();
    }
}
