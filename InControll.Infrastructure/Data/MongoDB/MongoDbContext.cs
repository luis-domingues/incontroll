using InControll.Domain.Entities;
using MongoDB.Driver;

namespace InControll.Infrastrucuture.Data.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        if(string.IsNullOrEmpty(connectionString)) throw new ArgumentException(nameof(connectionString));
        if(string.IsNullOrEmpty(databaseName)) throw new ArgumentException(nameof(databaseName));
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Transaction> Transactions
    {
        get { return _database.GetCollection<Transaction>("Transactions"); }
    }
}