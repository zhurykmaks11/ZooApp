using MongoDB.Driver;
using System.Collections.Generic;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class SupplierService
    {
        private readonly IMongoCollection<Supplier> _suppliers;

        public SupplierService(MongoDbContext context)
        {
            _suppliers = context.Suppliers;
        }

        // Отримати всіх постачальників
        public List<Supplier> GetAll()
        {
            return _suppliers.Find(_ => true).ToList();
        }

        // Додати постачальника
        public void Add(Supplier supplier)
        {
            _suppliers.InsertOne(supplier);
        }

        // Оновити
        public void Update(Supplier supplier)
        {
            _suppliers.ReplaceOne(s => s.Id == supplier.Id, supplier);
        }

        // Видалити
        public void Delete(string id)
        {
            _suppliers.DeleteOne(s => s.Id == id);
        }

        // Пошук
        public List<Supplier> Search(string keyword)
        {
            keyword = keyword.ToLower();

            return _suppliers.Find(s =>
                s.Name.ToLower().Contains(keyword) ||
                s.Address.ToLower().Contains(keyword) ||
                s.Phone.ToLower().Contains(keyword) ||
                s.FeedTypes.Any(f => f.ToLower().Contains(keyword))
            ).ToList();
        }
    }
}