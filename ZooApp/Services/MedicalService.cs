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

        
        public List<MedicalRecord> GetAllRecords()
        {
            return _records.Find(_ => true).ToList();
        }

        
        public void AddMedicalRecord(MedicalRecord record)
        {
            _records.InsertOne(record);

            
            _animals.UpdateOne(
                a => a.Id == record.AnimalId,
                Builders<Animal>.Update.Set(a => a.MedicalRecordId, record.Id)
            );
        }
        
        public void AddCheckup(string recordId, Checkup checkup)
        {
            var update = Builders<MedicalRecord>.Update.Push(r => r.Checkups, checkup);
            _records.UpdateOne(r => r.Id == recordId, update);
        }
        
        public void UpdateLatestCheckup(string recordId, Checkup updatedCheckup)
        {
            var record = _records.Find(r => r.Id == recordId).FirstOrDefault();
            if (record == null || record.Checkups.Count == 0)
                return;
            
            var ordered = record.Checkups
                .OrderBy(c => c.Date)
                .ToList();

            ordered[ordered.Count - 1] = updatedCheckup;
            record.Checkups = ordered;

            _records.ReplaceOne(r => r.Id == recordId, record);
        }
        
        public void DeleteRecord(string id)
        {
            _records.DeleteOne(r => r.Id == id);
            
            _animals.UpdateOne(
                a => a.MedicalRecordId == id,
                Builders<Animal>.Update.Set(a => a.MedicalRecordId, null)
            );
        }
        
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
