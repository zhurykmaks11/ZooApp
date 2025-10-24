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

        // 🐘 Тварини
        public IMongoCollection<Animal> Animals =>
            _database.GetCollection<Animal>("Animals");

        // 🏠 Клітки
        public IMongoCollection<Cage> Cages =>
            _database.GetCollection<Cage>("Cages");

        // 👨‍🔧 Працівники
        public IMongoCollection<Employee> Employees =>
            _database.GetCollection<Employee>("Employees");

        // 🔑 Користувачі (Keys)
        public IMongoCollection<KeyUser> KeyUsers =>
            _database.GetCollection<KeyUser>("Keys");

        // 🌱 КормAnimals
        public IMongoCollection<Feed> Feeds =>
            _database.GetCollection<Feed>("Feeds");

        // 🚚 Постачальники
        public IMongoCollection<Supplier> Suppliers =>
            _database.GetCollection<Supplier>("Suppliers");

        // 🩺 Медичні записи
        public IMongoCollection<MedicalRecord> MedicalRecords =>
            _database.GetCollection<MedicalRecord>("MedicalRecords");

        // 🍽 Графік годувань
        public IMongoCollection<FeedingSchedule> FeedingSchedules =>
            _database.GetCollection<FeedingSchedule>("FeedingsSchedules");

        // 🔄 Обміни зоопарків
        public IMongoCollection<Exchange> Exchanges =>
            _database.GetCollection<Exchange>("Exchanges");
    }