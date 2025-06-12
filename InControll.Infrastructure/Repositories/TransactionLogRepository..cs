using InControll.Domain;
using InControll.Domain.Entities;
using InControll.Infrastrucuture.Data.MongoDB;
using MongoDB.Driver;

namespace InControll.Infrastrucuture.Repositories;

public class TransactionLogRepository  : ITransactionLogRepository
{
    private readonly IMongoCollection<Transaction> _transactionsCollection;

    public TransactionLogRepository(MongoDbContext mongoDbContext)
    {
        _transactionsCollection = mongoDbContext.Transactions;
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _transactionsCollection.InsertOneAsync(transaction);
    }

    public async Task<Transaction?> GetByTransactionIdAsync(Guid transactionId)
    {
        var filter = Builders<Transaction>.Filter.Eq(t => t.Id, transactionId);
        return await _transactionsCollection.Find(filter).FirstOrDefaultAsync();
    }
}