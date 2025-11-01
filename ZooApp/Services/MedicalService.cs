using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class MedicalService
    {
        private readonly IMongoCollection<MedicalRecord> _medicalCollection;
        private readonly IMongoCollection<Animal> _animals;

        public MedicalService(MongoDbContext context)
        {
            _medicalCollection = context.MedicalRecords;
            _animals = context.Animals;
        }

        // ✅ Отримати всі медичні записи
        public List<MedicalRecord> GetAllRecords()
        {
            return _medicalCollection.Find(_ => true).ToList();
        }

        // ✅ Отримати останні обстеження (для таблиці)
        public List<(string AnimalName, DateTime Date, double Weight, double Height)> GetLatestCheckups()
        {
            var animals = _animals.Find(_ => true).ToList();
            var records = _medicalCollection.Find(_ => true).ToList();

            return records.Select(r =>
            {
                // 🩺 Шукаємо тварину по рядковому ID
                var animal = animals.FirstOrDefault(a => a.Id.ToString() == r.AnimalId);

                var lastCheck = r.Checkups.OrderByDescending(c => c.Date).FirstOrDefault();

                return (
                    animal?.Name ?? "Unknown",
                    lastCheck?.Date ?? DateTime.MinValue,
                    lastCheck?.Weight ?? 0,
                    lastCheck?.Height ?? 0
                );
            }).ToList();
        }

        // ✅ Додати новий медичний запис
        public void AddMedicalRecord(MedicalRecord record)
        {
            _medicalCollection.InsertOne(record);
        }

        // ✅ Додати новий огляд до існуючого запису
        public void AddCheckup(string recordId, Checkup checkup)
        {
            if (!ObjectId.TryParse(recordId, out var objectId))
                return;

            var update = Builders<MedicalRecord>.Update.Push(r => r.Checkups, checkup);
            _medicalCollection.UpdateOne(r => r.Id == objectId, update);
        }

        // ✅ Видалити медичний запис
        public void DeleteRecord(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            _medicalCollection.DeleteOne(r => r.Id == objectId);
        }

        // ✅ Знайти записи за назвою тварини
        public List<MedicalRecord> GetRecordsByAnimal(string animalName)
        {
            var animals = _animals
                .Find(a => a.Name.ToLower().Contains(animalName.ToLower()))
                .ToList();

            var animalIds = animals.Select(a => a.Id.ToString()).ToList();

            return _medicalCollection
                .Find(r => animalIds.Contains(r.AnimalId))
                .ToList();
        }

        // ✅ Пошук за хворобою або щепленням
        public List<MedicalRecord> SearchByDiseaseOrVaccine(string keyword)
        {
            keyword = keyword.ToLower();

            var allRecords = _medicalCollection.Find(_ => true).ToList();

            return allRecords
                .Where(r => r.Checkups.Any(c =>
                    c.Vaccinations.Any(v => v.ToLower().Contains(keyword)) ||
                    c.Illnesses.Any(i => i.ToLower().Contains(keyword))
                ))
                .ToList();
        }

        // ✅ Статистика за щепленнями
        public Dictionary<string, int> GetVaccinationStatistics()
        {
            var records = _medicalCollection.Find(_ => true).ToList();

            return records
                .SelectMany(r => r.Checkups)
                .SelectMany(c => c.Vaccinations)
                .GroupBy(v => v)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}
