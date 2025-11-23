using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooApp.Data;
using ZooApp.Models;
namespace ZooApp.Services
{
    public class FeedingService
    {
        private readonly IMongoCollection<FeedingSchedule> _feedingCollection;
        private readonly IMongoCollection<Animal> _animalsCollection;
        private readonly IMongoCollection<Feed> _feedsCollection;

        public FeedingService(MongoDbContext context)
        {
            _feedingCollection = context.FeedingSchedules;
            _animalsCollection = context.Animals;
            _feedsCollection = context.Feeds;
        }

        // ✅ 1. Отримати всі записи про годування
        public List<FeedingSchedule> GetAllFeedings()
        {
            return _feedingCollection.Find(_ => true).ToList();
        }

        public void AddFeeding(FeedingSchedule schedule)
        {
            try
            {
                var exists = _feedingCollection.Find(f =>
                    f.AnimalId == schedule.AnimalId &&
                    f.FeedingTime == schedule.FeedingTime &&
                    f.Season == schedule.Season).FirstOrDefault();

                if (exists != null)
                    throw new Exception("Feeding schedule for this animal and time already exists!");

                _feedingCollection.InsertOne(schedule);
            }
            catch (Exception ex)
            {
                // Тут відловлюємо будь-яку помилку
                throw new Exception($"Error adding feeding record: {ex.Message}");
            }
        }



        // ✅ 3. Редагувати запис
        public void UpdateFeeding(FeedingSchedule updated)
        {
            _feedingCollection.ReplaceOne(f => f.Id == updated.Id, updated);
        }

        // ✅ 4. Видалити запис
        public void DeleteFeeding(string id)
        {
            _feedingCollection.DeleteOne(f => f.Id == id);
        }

        // ✅ 5. Пошук годувань за твариною або типом корму
        public List<FeedingSchedule> SearchFeedings(string keyword)
        {
            keyword = keyword.ToLower();
            return _feedingCollection.Find(f =>
                f.AnimalName.ToLower().Contains(keyword) ||
                f.FeedType.ToLower().Contains(keyword)).ToList();
        }

        // ✅ 6. Отримати список тварин, які потребують певного типу корму у певний сезон
        public List<FeedingSchedule> GetAnimalsByFeedAndSeason(string feedType, string season)
        {
            var query = _feedingCollection.AsQueryable()
                .Where(f => f.FeedType.ToLower() == feedType.ToLower());

            if (!string.IsNullOrEmpty(season))
                query = query.Where(f => f.Season.ToLower() == season.ToLower());

            return query.ToList();
        }

        // ✅ 7. Отримати загальну кількість корму за сезоном
        public double GetTotalFeedBySeason(string season)
        {
            var feedings = _feedingCollection.AsQueryable()
                .Where(f => f.Season.ToLower() == season.ToLower());

            return feedings.Sum(f => (double)f.QuantityKg);
        }

        // ✅ 8. Отримати статистику годувань по типах кормів
        public Dictionary<string, double> GetFeedTypeStatistics()
        {
            var feedings = _feedingCollection.AsQueryable();

            return feedings
                .GroupBy(f => f.FeedType)
                .ToDictionary(g => g.Key, g => g.Sum(f => (double)f.QuantityKg));
        }

        // ✅ 9. Отримати повний список тварин із інформацією про їхній корм
        public List<FeedInfo> GetAnimalFeedInfo()
        {
            return _feedingCollection.AsQueryable()
                .Select(f => new FeedInfo
                {
                    AnimalName = f.AnimalName,
                    FeedType = f.FeedType,
                    Quantity = f.QuantityKg
                })
                .ToList();
        }


        // ✅ 10. Отримати всі унікальні сезони годування
        public List<string> GetAllSeasons()
        {
            return _feedingCollection.AsQueryable()
                .Select(f => f.Season)
                .Distinct()
                .ToList();
        }
        public int CleanupOrphanFeedings()
        {
            var animalIds = _animalsCollection.AsQueryable().Select(a => a.Id).ToList();
            var orphaned = _feedingCollection.Find(f => !animalIds.Contains(f.AnimalId)).ToList();

            foreach (var rec in orphaned)
                _feedingCollection.DeleteOne(f => f.Id == rec.Id);

            return orphaned.Count;
        }

        
    }
}
