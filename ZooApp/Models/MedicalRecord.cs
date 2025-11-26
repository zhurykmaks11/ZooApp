using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ZooApp.Models
{
    public class Checkup
    {
        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("weight")]
        public double Weight { get; set; }

        [BsonElement("height")]
        public double Height { get; set; }

        [BsonElement("vaccinations")]
        public List<string> Vaccinations { get; set; } = new();

        [BsonElement("illnesses")]
        public List<string> Illnesses { get; set; } = new();

        [BsonElement("treatment")]
        public string Treatment { get; set; }
    }

    public class MedicalRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("animalId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AnimalId { get; set; }

        [BsonElement("checkups")]
        public List<Checkup> Checkups { get; set; } = new();
    }
}