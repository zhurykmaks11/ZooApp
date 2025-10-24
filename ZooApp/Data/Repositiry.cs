using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZooApp.Data
{
    public class Repository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public Repository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        // ➕ Створити
        public async Task InsertAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        // 📄 Отримати всі
        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // 🔎 Отримати за Id
        public async Task<T> GetByIdAsync(ObjectId id)
        {
            return await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
        }

        // ✏️ Оновити
        public async Task UpdateAsync(ObjectId id, T entity)
        {
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity);
        }

        // ❌ Видалити
        public async Task DeleteAsync(ObjectId id)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
        }
    }
}