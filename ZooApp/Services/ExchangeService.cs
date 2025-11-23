using MongoDB.Driver;
using System.Collections.Generic;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class ExchangeService
    {
        private readonly IMongoCollection<ExchangeRecord> _exchangeCollection;

        public ExchangeService(MongoDbContext context)
        {
            _exchangeCollection = context.ExchangeRecords;
        }

        // 📌 Отримати всі записи
        public List<ExchangeRecord> GetAll()
        {
            return _exchangeCollection.Find(_ => true)
                .SortByDescending(e => e.ExchangeDate)
                .ToList();
        }

        // 📌 Додати новий запис
        public void Add(ExchangeRecord record)
        {
            _exchangeCollection.InsertOne(record);
        }

        // 📌 Оновити існуючий
        public void Update(ExchangeRecord record)
        {
            _exchangeCollection.ReplaceOne(r => r.Id == record.Id, record);
        }

        // 📌 Видалити запис
        public void Delete(string id)
        {
            _exchangeCollection.DeleteOne(r => r.Id == id);
        }

        // 📌 Пошук (за ім’ям тварини, типом, зоопарком, причиною)
        public List<ExchangeRecord> Search(string keyword)
        {
            keyword = keyword.ToLower();

            return _exchangeCollection.Find(r =>
                r.AnimalName.ToLower().Contains(keyword) ||
                r.ExchangeType.ToLower().Contains(keyword) ||
                r.OtherZoo.ToLower().Contains(keyword) ||
                r.Reason.ToLower().Contains(keyword)
            ).ToList();
        }

        // 📌 Отримати всі записи по конкретній тварині
        public List<ExchangeRecord> GetByAnimal(string animalId)
        {
            return _exchangeCollection.Find(r => r.AnimalId == animalId).ToList();
        }
    }
}