using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ZooApp.Models
{
    public class ExchangeRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("animalId")]
        public string AnimalId { get; set; }

        [BsonElement("exchangeDate")]
        public DateTime ExchangeDate { get; set; }

        [BsonElement("animalName")]
        public string AnimalName { get; set; }

        [BsonElement("exchangeType")]  
        public string ExchangeType { get; set; } 

        [BsonElement("otherZoo")]
        public string OtherZoo { get; set; }

        [BsonElement("reason")]
        public string Reason { get; set; }
    }
}