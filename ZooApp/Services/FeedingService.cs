using MongoDB.Bson;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class FeedingService
    {
        private readonly IMongoCollection<FeedingSchedule> _feedingSchedules;

        public FeedingService(MongoDbContext context)
        {
            _feedingSchedules = context.FeedingSchedules;
        }

        // ➕ Додати графік годування
        public void AddSchedule(FeedingSchedule schedule)
        {
            _feedingSchedules.InsertOne(schedule);
        }

        // 🔍 Отримати графік по тварині
        public FeedingSchedule GetByAnimalId(string animalId)
        {
            return _feedingSchedules.Find(s => s.AnimalId == new MongoDB.Bson.ObjectId(animalId)).FirstOrDefault();
        }

        // 📊 Підрахунок кормів на день
        public double CalculateDailyFeed(ObjectId feedId)
        {
            var schedules = _feedingSchedules.Find(_ => true).ToList();
            return schedules.SelectMany(s => s.Meals)
                .Where(m => m.FeedId == feedId)
                .Sum(m => m.AmountKg);
        }
    }
}