using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class CagesService
    {
        private readonly IMongoCollection<Cage> _cages;
        private readonly IMongoCollection<Animal> _animals;

        public CagesService(MongoDbContext context)
        {
            _cages = context.Cages;
            _animals = context.Animals;
        }

        // ✅ Отримати всі клітки
        public List<Cage> GetAllCages()
        {
            return _cages.Find(_ => true).ToList();
        }

        // ✅ Додати клітку
        public void AddCage(Cage cage)
        {
            _cages.InsertOne(cage);
        }

        // ✅ Перевірити чи тварина може бути поселена
        private bool CanAssignAnimal(Cage cage, Animal animal)
        {
            // 1) Перевірка місця
            if (cage.Animals.Count >= cage.Capacity)
                return false;

            var allAnimals = _animals.Find(_ => true).ToList();
            
            var cageAnimals = allAnimals.Where(a =>
                !string.IsNullOrEmpty(a.Id) &&
                cage.Animals.Contains(ObjectId.Parse(a.Id))
            ).ToList();


            // 2) Хижак + травоїдний — заборонено
            if (animal.Type == "predator" && cageAnimals.Any(a => a.Type == "herbivore"))
                return false;

            if (animal.Type == "herbivore" && cageAnimals.Any(a => a.Type == "predator"))
                return false;

            // перевірка видів без регістру + часткове входження
            bool speciesMatch = cage.CompatibleSpecies
                .Any(s => animal.Species.ToLower().Contains(s.ToLower()));

            if (!speciesMatch)
                return false;


            return true;
        }
        
        public void UpdateCage(string cageId, string location, string size, int capacity, List<string> species)
        {
            var oid = ObjectId.Parse(cageId);

            var update = Builders<Cage>.Update
                .Set(c => c.Location, location)
                .Set(c => c.Size, size)
                .Set(c => c.Capacity, capacity)
                .Set(c => c.CompatibleSpecies, species);

            _cages.UpdateOne(c => c.Id == oid, update);
        }

        public Cage GetCageById(string cageId)
        {
            var oid = ObjectId.Parse(cageId);
            return _cages.Find(c => c.Id == oid).FirstOrDefault();
        }
        
        public void DeleteCage(string cageId)
        {
            var oid = ObjectId.Parse(cageId);
            _cages.DeleteOne(c => c.Id == oid);
        }

        public bool MoveAnimal(string animalId, string newCageId)
        {
            var animal = _animals.Find(a => a.Id == animalId).FirstOrDefault();
            if (animal == null) return false;

            string oldCageId = animal.CageId;

            // Якщо тварина вже в цій клітці — нічого не робимо
            if (oldCageId == newCageId)
                return false;

            // 1️⃣ Знімаємо тварину зі старої клітки
            if (oldCageId != null)
            {
                RemoveAnimalFromCage(animalId, oldCageId);
            }

            // 2️⃣ Пробуємо поселити в нову
            bool success = AddAnimalToCage(animalId, newCageId);

            // Якщо не вийшло — повертаємо назад в стару клітку
            if (!success)
            {
                if (oldCageId != null)
                    AddAnimalToCage(animalId, oldCageId);

                return false;
            }

            return true;
        }


        // ✅ Додати тварину в клітку
        public bool AddAnimalToCage(string animalId, string cageId)
        {
            var cageObjId = ObjectId.Parse(cageId);
            var animalObjId = ObjectId.Parse(animalId);

            var cage = _cages.Find(c => c.Id == cageObjId).FirstOrDefault();
            var animal = _animals.Find(a => a.Id == animalId).FirstOrDefault();

            if (cage == null || animal == null)
                return false;

            if (!CanAssignAnimal(cage, animal))
                return false;

            // Додаємо тварину
            var update = Builders<Cage>.Update.Push(c => c.Animals, animalObjId);
            _cages.UpdateOne(c => c.Id == cageObjId, update);

            // Оновлюємо тварину (зв'язуємо)
            var updateAnimal = Builders<Animal>.Update.Set(a => a.CageId, cageId);
            _animals.UpdateOne(a => a.Id == animalId, updateAnimal);

            return true;
        }

        // ✅ Видалити тварину з клітки
        public void RemoveAnimalFromCage(string animalId, string cageId)
        {
            var cageObjId = ObjectId.Parse(cageId);
            var animalObjId = ObjectId.Parse(animalId);

            var update = Builders<Cage>.Update.Pull(c => c.Animals, animalObjId);
            _cages.UpdateOne(c => c.Id == cageObjId, update);

            var updateAnimal = Builders<Animal>.Update.Set(a => a.CageId, null);
            _animals.UpdateOne(a => a.Id == animalId, updateAnimal);
        }
    }
}
