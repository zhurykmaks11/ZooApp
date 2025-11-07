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

        // ✅ Отримати всіх тварин
        public List<Animal> GetAllAnimals()
        {
            return _animals.Find(_ => true).ToList();
        }

        // ✅ Додати тварину
        public void AddAnimal(Animal animal)
        {
            _animals.InsertOne(animal);
        }

        // ✅ Оновити тварину
        public void UpdateAnimal(Animal animal)
        {
            _animals.ReplaceOne(a => a.Id == animal.Id, animal);
        }

        // ✅ Видалити тварину
        public void DeleteAnimal(string id)
        {
            _animals.DeleteOne(a => a.Id == id);
        }
    }
}