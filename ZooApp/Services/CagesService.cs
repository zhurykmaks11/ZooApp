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

        
        public List<Cage> GetAllCages()
        {
            return _cages.Find(_ => true).ToList();
        }

        
        public Cage GetCage(string cageId)
        {
            if (!ObjectId.TryParse(cageId, out var id)) return null;
            return _cages.Find(c => c.Id == id).FirstOrDefault();
        }

        
        public void AddCage(Cage cage) => _cages.InsertOne(cage);

       
        public void DeleteCage(string cageId)
        {
            if (!ObjectId.TryParse(cageId, out var id)) return;
            _cages.DeleteOne(c => c.Id == id);
        }

        
        public void UpdateCage(string cageId, string location, string size, int cap, List<string> species)
        {
            if (!ObjectId.TryParse(cageId, out var id)) return;

            var upd = Builders<Cage>.Update
                .Set(c => c.Location, location)
                .Set(c => c.Size, size)
                .Set(c => c.Capacity, cap)
                .Set(c => c.CompatibleSpecies, species);

            _cages.UpdateOne(c => c.Id == id, upd);
        }
        
        public bool CanAssignAnimalSafe(string cageId, Animal animal)
        {
            if (!ObjectId.TryParse(cageId, out var cageObjId))
                return false;

            var cage = _cages.Find(c => c.Id == cageObjId).FirstOrDefault();
            if (cage == null) return false;

           
            if (cage.Animals.Count >= cage.Capacity)
                return false;

           
            if (cage.CompatibleSpecies.Count > 0 &&
                !cage.CompatibleSpecies.Any(s => s.ToLower() == animal.Species.ToLower()))
                return false;

           
            var inside = _animals.Find(a => a.CageId == cageId).ToList();

            if (animal.Type == "predator" && inside.Any(a => a.Type == "herbivore"))
                return false;

            if (animal.Type == "herbivore" && inside.Any(a => a.Type == "predator"))
                return false;

            
            if (cage.AllowedTypes.Count > 0 &&
                !cage.AllowedTypes.Contains(animal.Type))
                return false;

            
            if (animal.NeedsWarmShelter && !cage.Heated)
                return false;

            
            foreach (var nId in cage.NeighborCageIds)
            {
                var neighbors = _animals.Find(a => a.CageId == nId).ToList();

                if (animal.Type == "predator" && neighbors.Any(a => a.Type == "herbivore"))
                    return false;

                if (animal.Type == "herbivore" && neighbors.Any(a => a.Type == "predator"))
                    return false;
            }

            return true;
        }

       
        public bool AddAnimalToCage(string animalId, string cageId)
        {
            if (!ObjectId.TryParse(animalId, out var aId)) return false;
            if (!ObjectId.TryParse(cageId, out var cId)) return false;

            var animal = _animals.Find(a => a.Id == animalId).FirstOrDefault();
            if (animal == null) return false;

            if (!CanAssignAnimalSafe(cageId, animal))
                return false;

            
            _cages.UpdateOne(c => c.Id == cId,
                Builders<Cage>.Update.Push(c => c.Animals, aId));

            
            _animals.UpdateOne(a => a.Id == animalId,
                Builders<Animal>.Update.Set(a => a.CageId, cageId));

            return true;
        }

        
        public void RemoveAnimalFromCage(string animalId, string cageId)
        {
            if (!ObjectId.TryParse(animalId, out var aId)) return;
            if (!ObjectId.TryParse(cageId, out var cId)) return;

            _cages.UpdateOne(c => c.Id == cId,
                Builders<Cage>.Update.Pull(c => c.Animals, aId));

            _animals.UpdateOne(a => a.Id == animalId,
                Builders<Animal>.Update.Set(a => a.CageId, null));
        }

        
        public bool MoveAnimal(string animalId, string newCageId)
        {
            var animal = _animals.Find(a => a.Id == animalId).FirstOrDefault();
            if (animal == null) return false;

            var oldCage = animal.CageId;

            if (newCageId == oldCage) return true;

            if (!CanAssignAnimalSafe(newCageId, animal))
                return false;

            if (!string.IsNullOrEmpty(oldCage))
                RemoveAnimalFromCage(animalId, oldCage);

            if (!string.IsNullOrEmpty(newCageId))
                return AddAnimalToCage(animalId, newCageId);

            return true;
        }
    }
}
