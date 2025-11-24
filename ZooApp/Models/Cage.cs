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

        [BsonElement("capacity")]
        public int Capacity { get; set; }

        // Сумісні види
        [BsonElement("compatibleSpecies")]
        public List<string> CompatibleSpecies { get; set; } = new();

        // Дозволені типи
        [BsonElement("allowedTypes")]
        public List<string> AllowedTypes { get; set; } = new();

        // Тварини в клітці
        [BsonElement("animals")]
        public List<ObjectId> Animals { get; set; } = new();

        // Сусідні клітки
        [BsonElement("neighborCageIds")]
        public List<string> NeighborCageIds { get; set; } = new();
    }
}