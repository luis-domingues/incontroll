using InControll.Domain.Entities;

namespace InControll.Domain;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task<Payment?> GetByIdAsync(Guid id);
    Task UpdateAsync(Payment payment);
}