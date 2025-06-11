namespace InControll.Domain;

public enum TransactionType
{
    Authorization = 1,
    Capture = 2,
    Refund = 3,
    Chargeback = 4,
    Void = 5,
}