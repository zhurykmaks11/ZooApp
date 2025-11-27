using MongoDB.Driver;

using ZooApp.Models;

namespace ZooApp.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string dbName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(dbName);
       

    }
    
    public IMongoCollection<Animal> Animals =>
        _database.GetCollection<Animal>("Animals");

    
    public IMongoCollection<Cage> Cages =>
        _database.GetCollection<Cage>("Cages");

    
    public IMongoCollection<Employee> Employees =>
        _database.GetCollection<Employee>("Employees");

    
    public IMongoCollection<KeyUser> KeyUsers =>
        _database.GetCollection<KeyUser>("Keys");

    
    public IMongoCollection<Feed> Feeds =>
        _database.GetCollection<Feed>("Feeds");


    public IMongoCollection<Supplier> Suppliers =>
        _database.GetCollection<Supplier>("Suppliers");

    
    public IMongoCollection<MedicalRecord> MedicalRecords =>
        _database.GetCollection<MedicalRecord>("MedicalRecords");


    public IMongoCollection<FeedingSchedule> FeedingSchedules =>
        _database.GetCollection<FeedingSchedule>("FeedingSchedule");


   
    public IMongoCollection<ExchangeRecord> ExchangeRecords =>
        _database.GetCollection<ExchangeRecord>("Exchange");
    
     public IMongoCollection<LogRecord> Logs =>
        _database.GetCollection<LogRecord>("Logs");


}