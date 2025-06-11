namespace InControll.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid PaymentId { get; private set; } // FK Payment
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string GatewayTransactionId { get; private set; } // Transaction Id on getway
    public string GatewayResponse { get; private set; } // resposta bruta do gateway (JSON/XML)
    public string Description { get; private set; }

    public Transaction(Guid paymentId, decimal amount, TransactionType type, TransactionStatus status, string gatewayTransactionId, string gatewayResponse, string description)
    {
        PaymentId = paymentId;
        Amount = amount;
        Type = type;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        GatewayTransactionId = gatewayTransactionId;
        GatewayResponse = gatewayResponse;
        Description = description;
    }
    
    private Transaction() { } // EF Core constructor

    public void UpdateStatus(TransactionStatus newStatus)
    {
        Status = newStatus;
    }
}