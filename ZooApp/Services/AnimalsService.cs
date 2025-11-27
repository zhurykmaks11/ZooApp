using MongoDB.Driver;
using System.Collections.Generic;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class AnimalsService
    {
        private readonly IMongoCollection<Animal> _animals;

        public AnimalsService(MongoDbContext context)
        {
            _animals = context.Animals;
        }

       
        public List<Animal> GetAllAnimals()
        {
            return _animals.Find(_ => true).ToList();
        }

       
        public void AddChild(string motherId, string fatherId, string childId)
        {
            if (!string.IsNullOrEmpty(motherId))
            {
                _animals.UpdateOne(
                    a => a.Id == motherId,
                    Builders<Animal>.Update.Push(a => a.ChildrenIds, childId)
                );
            }

            if (!string.IsNullOrEmpty(fatherId))
            {
                _animals.UpdateOne(
                    a => a.Id == fatherId,
                    Builders<Animal>.Update.Push(a => a.ChildrenIds, childId)
                );
            }
        }

        
        public void AddAnimal(Animal animal)
        {
            _animals.InsertOne(animal);
        }

       
        public void UpdateAnimal(Animal animal)
        {
            _animals.ReplaceOne(a => a.Id == animal.Id, animal);
        }

        
        public void DeleteAnimal(string id)
        {
            _animals.DeleteOne(a => a.Id == id);
        }
    }
}