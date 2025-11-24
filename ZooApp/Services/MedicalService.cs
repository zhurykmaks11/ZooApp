using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class MedicalService
    {
        private readonly IMongoCollection<MedicalRecord> _records;
        private readonly IMongoCollection<Animal> _animals;

        public MedicalService(MongoDbContext context)
        {
            _records = context.MedicalRecords;
            _animals = context.Animals;
        }

        // ✅ Отримати всі медичні записи
        public List<MedicalRecord> GetAllRecords()
        {
            return _records.Find(_ => true).ToList();
        }

        // ✅ Додати нову медичну картку
        public void AddMedicalRecord(MedicalRecord record)
        {
            _records.InsertOne(record);

            // оновлюємо посилання в тварині (якщо є поле MedicalRecordId)
            _animals.UpdateOne(
                a => a.Id == record.AnimalId,
                Builders<Animal>.Update.Set(a => a.MedicalRecordId, record.Id)
            );
        }

        // ✅ Додати checkup до існуючої картки
        public void AddCheckup(string recordId, Checkup checkup)
        {
            var update = Builders<MedicalRecord>.Update.Push(r => r.Checkups, checkup);
            _records.UpdateOne(r => r.Id == recordId, update);
        }

        // ✅ Оновити останній checkup у картці
        public void UpdateLatestCheckup(string recordId, Checkup updatedCheckup)
        {
            var record = _records.Find(r => r.Id == recordId).FirstOrDefault();
            if (record == null || record.Checkups.Count == 0)
                return;

            // вважаємо, що останній — по даті
            var ordered = record.Checkups
                .OrderBy(c => c.Date)
                .ToList();

            ordered[ordered.Count - 1] = updatedCheckup;
            record.Checkups = ordered;

            _records.ReplaceOne(r => r.Id == recordId, record);
        }

        // ✅ Видалити медичну картку
        public void DeleteRecord(string id)
        {
            _records.DeleteOne(r => r.Id == id);

            // можна за бажанням очистити MedicalRecordId в тварині
            _animals.UpdateOne(
                a => a.MedicalRecordId == id,
                Builders<Animal>.Update.Set(a => a.MedicalRecordId, null)
            );
        }

        // ✅ Пошук по хворобах / вакцинаціях (для Find)
        public List<MedicalRecord> SearchByDiseaseOrVaccine(string query)
        {
            query = query.ToLower();

            var all = _records.Find(_ => true).ToList();

            return all.Where(r =>
                r.Checkups.Any(c =>
                    c.Vaccinations.Any(v => v.ToLower().Contains(query)) ||
                    c.Illnesses.Any(i => i.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(c.Treatment) &&
                     c.Treatment.ToLower().Contains(query))
                )
            ).ToList();
        }

        // ✅ Статистика вакцинацій
        public Dictionary<string, int> GetVaccinationStatistics()
        {
            var all = _records.Find(_ => true).ToList();

            return all
                .SelectMany(r => r.Checkups)
                .SelectMany(c => c.Vaccinations)
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .GroupBy(v => v)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        
        public MedicalRecord GetRecord(string id)
        {
            return _records.Find(r => r.Id == id).FirstOrDefault();
        }

    }
}
