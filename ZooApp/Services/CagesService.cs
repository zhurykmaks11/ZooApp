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

        // -------------------------
        // BASIC CRUD
        // -------------------------
        public List<Cage> GetAllCages()
        {
            return _cages.Find(_ => true).ToList();
        }

        public Cage GetCage(string cageId)
        {
            if (!ObjectId.TryParse(cageId, out ObjectId id))
                return null;

            return _cages.Find(c => c.Id == id).FirstOrDefault();
        }

        public void AddCage(Cage cage)
        {
            _cages.InsertOne(cage);
        }

        public void UpdateCage(string cageId, string location, string size, int capacity, List<string> species)
        {
            if (!ObjectId.TryParse(cageId, out ObjectId oid))
                return;

            var update = Builders<Cage>.Update
                .Set(c => c.Location, location)
                .Set(c => c.Size, size)
                .Set(c => c.Capacity, capacity)
                .Set(c => c.CompatibleSpecies, species);

            _cages.UpdateOne(c => c.Id == oid, update);
        }

        public void DeleteCage(string cageId)
        {
            if (!ObjectId.TryParse(cageId, out ObjectId oid))
                return;

            _cages.DeleteOne(c => c.Id == oid);
        }

        // -------------------------
        // SAFE CHECK BEFORE MOVING
        // -------------------------
        public bool CanAssignAnimalSafe(string cageId, Animal animal)
        {
            if (!ObjectId.TryParse(cageId, out ObjectId cageObjId))
                return false;

            var cage = _cages.Find(c => c.Id == cageObjId).FirstOrDefault();
            if (cage == null) return false;

            // 1. Capacity
            if (cage.Animals.Count >= cage.Capacity)
                return false;

            // 2. Species compatibility (case-insensitive)
            if (cage.CompatibleSpecies.Count > 0 &&
                !cage.CompatibleSpecies.Any(s =>
                    s.ToLower().Trim() == animal.Species.ToLower().Trim()))
                return false;

            // 3. Type rule
            if (cage.AllowedTypes.Count > 0 &&
                !cage.AllowedTypes.Contains(animal.Type))
                return false;

            // 4. Fetch animals in cage
            var animalsInCage = _animals.Find(a =>
                a.CageId == cageId).ToList();

            // 5. Predator-herbivore conflict
            if (animal.Type == "predator" && animalsInCage.Any(a => a.Type == "herbivore"))
                return false;

            if (animal.Type == "herbivore" && animalsInCage.Any(a => a.Type == "predator"))
                return false;

            return true;
        }

        // -------------------------
        // MOVE ANIMAL BETWEEN CAGES
        // -------------------------
        public bool MoveAnimal(string animalId, string newCageId)
        {
            var animal = _animals.Find(a => a.Id == animalId).FirstOrDefault();
            if (animal == null)
                return false;

            string oldCage = animal.CageId;

            // No change
            if (oldCage == newCageId)
                return true;

            // Check BEFORE removal from old cage
            if (!string.IsNullOrEmpty(newCageId) &&
                !CanAssignAnimalSafe(newCageId, animal))
            {
                return false;
            }

            // ✔ Remove from old cage (if existed)
            if (!string.IsNullOrEmpty(oldCage))
                RemoveAnimalFromCage(animalId, oldCage);

            // ✔ Add to new cage
            if (!string.IsNullOrEmpty(newCageId))
            {
                if (!AddAnimalToCage(animalId, newCageId))
                {
                    // If fail → return to old cage
                    if (!string.IsNullOrEmpty(oldCage))
                        AddAnimalToCage(animalId, oldCage);

                    return false;
                }
            }

            return true;
        }

        // -------------------------
        // ASSIGN TO CAGE
        // -------------------------
        public bool AddAnimalToCage(string animalId, string cageId)
        {
            if (!ObjectId.TryParse(animalId, out ObjectId animalObjId)) return false;
            if (!ObjectId.TryParse(cageId, out ObjectId cageObjId)) return false;

            var cage = _cages.Find(c => c.Id == cageObjId).FirstOrDefault();
            var animal = _animals.Find(a => a.Id == animalId).FirstOrDefault();

            if (cage == null || animal == null)
                return false;

            // Final check using unified logic
            if (!CanAssignAnimalSafe(cageId, animal))
                return false;

            // Add animal reference
            var updateCage = Builders<Cage>.Update.Push(c => c.Animals, animalObjId);
            _cages.UpdateOne(c => c.Id == cageObjId, updateCage);

            // Link animal to cage
            var updateAnimal = Builders<Animal>.Update.Set(a => a.CageId, cageId);
            _animals.UpdateOne(a => a.Id == animalId, updateAnimal);

            return true;
        }

        // -------------------------
        // REMOVE FROM CAGE
        // -------------------------
        public void RemoveAnimalFromCage(string animalId, string cageId)
        {
            if (!ObjectId.TryParse(animalId, out ObjectId animalObjId)) return;
            if (!ObjectId.TryParse(cageId, out ObjectId cageObjId)) return;

            var update = Builders<Cage>.Update.Pull(c => c.Animals, animalObjId);
            _cages.UpdateOne(c => c.Id == cageObjId, update);

            var updateAnimal = Builders<Animal>.Update.Set(a => a.CageId, null);
            _animals.UpdateOne(a => a.Id == animalId, updateAnimal);
        }
    }
}
