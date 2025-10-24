using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class ExchangeService
    {
        private readonly IMongoCollection<Exchange> _exchanges;

        public ExchangeService(MongoDbContext context)
        {
            _exchanges = context.Exchanges;
        }

        // ➕ Додати запис обміну
        public void AddExchange(Exchange exchange)
        {
            _exchanges.InsertOne(exchange);
        }

        // 🔍 Знайти обміни по тварині
        public List<Exchange> GetByAnimalId(string animalId)
        {
            return _exchanges.Find(e => e.AnimalId == new MongoDB.Bson.ObjectId(animalId)).ToList();
        }

        // 📃 Усі обміни
        public List<Exchange> GetAll()
        {
            return _exchanges.Find(_ => true).ToList();
        }
    }
}