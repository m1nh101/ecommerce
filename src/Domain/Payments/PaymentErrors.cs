using Domain.Primitives;

namespace Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error NotFound = new("Payment.NotFound", "The payment was not found.");
    public static readonly Error CannotComplete = new("Payment.CannotComplete", "Only processing payments can be completed.");
    public static readonly Error CannotFail = new("Payment.CannotFail", "Only processing payments can be failed.");
    public static readonly Error CannotRefund = new("Payment.CannotRefund", "Only completed payments can be refunded.");
    public static readonly Error CannotProcess = new("Payment.CannotProcess", "Only pending payments can be moved to processing.");
    public static readonly Error MissingTransactionId = new("Payment.MissingTransactionId", "Transaction ID is required to complete a payment.");
    public static readonly Error MissingFailureReason = new("Payment.MissingFailureReason", "A failure reason is required.");
    public static readonly Error OrderAlreadyHasPayment = new("Payment.OrderAlreadyHasPayment", "A payment already exists for this order.");
}
