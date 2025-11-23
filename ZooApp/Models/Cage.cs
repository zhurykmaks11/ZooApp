using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ZooApp.Models
{
    public class Cage
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string IdString => Id.ToString();

        [BsonElement("number")]
        public int Number { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("size")]
        public string Size { get; set; }

        [BsonElement("heated")]
        public bool Heated { get; set; } 

        // Максимальна кількість тварин
        [BsonElement("capacity")]
        public int Capacity { get; set; }

        // Список сумісних видів (напр. "слон", "бегемот")
        [BsonElement("compatibleSpecies")]
        public List<string> CompatibleSpecies { get; set; } = new();

        // Список ID тварин, які зараз у клітці
        [BsonElement("animals")]
        public List<ObjectId> Animals { get; set; } = new();
        
        // опційно: дозволені типи ("herbivore"/"carnivore")
        [BsonElement("allowedTypes")]
        public List<string> AllowedTypes { get; set; } = new();
    }
}