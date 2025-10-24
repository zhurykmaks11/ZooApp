using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ZooApp.Models
{
    public class Animal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("species")]
        public string Species { get; set; }

        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("birthDate")]
        public DateTime BirthDate { get; set; }

        [BsonElement("weight")]
        public double Weight { get; set; }

        [BsonElement("height")]
        public double Height { get; set; }

        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("climateZone")]
        public string ClimateZone { get; set; }

        [BsonElement("cageId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CageId { get; set; }

        [BsonElement("feedingScheduleId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FeedingScheduleId { get; set; }

        [BsonElement("medicalRecordId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MedicalRecordId { get; set; }
    }
}