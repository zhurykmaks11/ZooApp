using MongoDB.Driver;
using System.Collections.Generic;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class ExchangeService
    {
        private readonly IMongoCollection<ExchangeRecord> _exchangeCollection;

        public ExchangeService(MongoDbContext context)
        {
            _exchangeCollection = context.ExchangeRecords;
        }
        
        public List<ExchangeRecord> GetAll()
        {
            return _exchangeCollection.Find(_ => true)
                .SortByDescending(e => e.ExchangeDate)
                .ToList();
        }

      
        public void Add(ExchangeRecord record)
        {
            _exchangeCollection.InsertOne(record);
        }
       
        public List<string> GetPartnerZoos()
        {
            return _exchangeCollection.AsQueryable()
                .Select(e => e.OtherZoo)
                .Distinct()
                .OrderBy(z => z)
                .ToList();
        }


        public List<string> GetPartnerZoosBySpecies(string species)
        {
            return _exchangeCollection.AsQueryable()
                .Where(e => e.AnimalName.ToLower().Contains(species.ToLower()))
                .Select(e => e.OtherZoo)
                .Distinct()
                .OrderBy(z => z)
                .ToList();
        }


       
        public void Update(ExchangeRecord record)
        {
            _exchangeCollection.ReplaceOne(r => r.Id == record.Id, record);
        }
        
        public void Delete(string id)
        {
            _exchangeCollection.DeleteOne(r => r.Id == id);
        }
        
        public List<ExchangeRecord> Search(string keyword)
        {
            keyword = keyword.ToLower();

            return _exchangeCollection.Find(r =>
                r.AnimalName.ToLower().Contains(keyword) ||
                r.ExchangeType.ToLower().Contains(keyword) ||
                r.OtherZoo.ToLower().Contains(keyword) ||
                r.Reason.ToLower().Contains(keyword)
            ).ToList();
        }
        
        public List<ExchangeRecord> GetByAnimal(string animalId)
        {
            return _exchangeCollection.Find(r => r.AnimalId == animalId).ToList();
        }
    }
}