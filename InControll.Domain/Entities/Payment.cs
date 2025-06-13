namespace InControll.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string PaymentMethod { get; private set; }
    public string CustomerId { get; private set; }
    public string OrderId { get; private set; }
    
    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public Payment(decimal amount, string currency, string customerId, string orderId, string paymentMethod)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));
        if (string.IsNullOrWhiteSpace(paymentMethod)) throw new ArgumentException("Payment method cannot be null or empty.", nameof(paymentMethod));
        if (string.IsNullOrWhiteSpace(customerId)) throw new ArgumentException("Customer ID cannot be null or empty.", nameof(customerId));
        if (string.IsNullOrWhiteSpace(orderId)) throw new ArgumentException("Order ID cannot be null or empty.", nameof(orderId));
        
        Id = Guid.NewGuid();
        Amount = amount;
        Currency = currency;
        CustomerId = customerId;
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }
    
    private Payment() { }

    public void AddTransaction(Transaction transaction)
    {
        if(transaction == null) throw new ArgumentNullException(nameof(transaction));
        if(transaction.PaymentId != this.Id) throw new ArgumentException("The transaction's payment ID is not the same as the transaction's payment ID.");
        
        _transactions.Add(transaction);

        if(transaction.Type == TransactionType.Authorization && transaction.Status == TransactionStatus.Success)
        {
            Status = PaymentStatus.Approved;
        }
        else if(transaction.Type == TransactionType.Capture && transaction.Status == TransactionStatus.Success)
        {
            Status = PaymentStatus.Approved;
        }
        else if(transaction.Type == TransactionType.Refund && transaction.Status == TransactionStatus.Success)
        {
            Status = CalculateRefundedAmount() >= Amount ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
        }
        else if(transaction.Status == TransactionStatus.Failed)
        {
            if(Status == PaymentStatus.Denied);
        }
    }

    public void UpdateStatus(PaymentStatus newStatus)
    {
        Status = newStatus;
    }

    private decimal CalculateRefundedAmount()
    {
        return _transactions.Where(t => t.Type == TransactionType.Refund && t.Status == TransactionStatus.Success).Sum(t => t.Amount);
    }
}