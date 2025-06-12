using InControll.Domain.Entities;

namespace InControll.Domain;

public interface ITransactionLogRepository
{
    Task AddAsync(Transaction transaction);
    Task<Transaction?> GetByTransactionIdAsync(Guid transactionId);
}