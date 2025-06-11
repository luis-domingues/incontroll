namespace InControll.Domain;

public enum PaymentStatus
{
    Pending = 1,
    Approved = 2,
    Denied = 3,
    Refunded = 4,
    PartiallyRefunded = 5,
    Cancelled = 6,
}