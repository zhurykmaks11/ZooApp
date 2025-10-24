using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class MedicalService
    {
        private readonly IMongoCollection<MedicalRecord> _medicalRecords;

        public MedicalService(MongoDbContext context)
        {
            _medicalRecords = context.MedicalRecords;
        }

        // ➕ Додати медичну картку
        public void AddMedicalRecord(MedicalRecord record)
        {
            _medicalRecords.InsertOne(record);
        }

        // 🔍 Отримати картку по тварині
        public MedicalRecord GetByAnimalId(string animalId)
        {
            return _medicalRecords.Find(r => r.AnimalId == new MongoDB.Bson.ObjectId(animalId)).FirstOrDefault();
        }

        // 📃 Усі картки
        public List<MedicalRecord> GetAll()
        {
            return _medicalRecords.Find(_ => true).ToList();
        }
    }
}
