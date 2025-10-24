using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models
{
    public class FeedingSchedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("animalId")]
        public string AnimalId { get; set; }

        [BsonElement("animalName")]  // ✅ важливо
        public string AnimalName { get; set; }

        [BsonElement("feedType")]
        public string FeedType { get; set; }

        [BsonElement("quantityKg")]
        public double QuantityKg { get; set; }

        [BsonElement("feedingTime")]
        public string FeedingTime { get; set; }

        [BsonElement("season")]
        public string Season { get; set; }
    }
}