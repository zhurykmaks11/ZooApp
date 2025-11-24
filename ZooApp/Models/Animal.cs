using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ZooApp.Models
{
    [BsonIgnoreExtraElements]
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

        /// <summary>
        /// "herbivore" / "predator"
        /// </summary>
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

        // 🔥 НОВІ ПОЛЯ — для зимівлі, ізоляції, родини

        [BsonElement("needsWarmShelter")]
        public bool NeedsWarmShelter { get; set; }

        [BsonElement("isIsolated")]
        public bool IsIsolated { get; set; }

        [BsonElement("isolationReason")]
        public string IsolationReason { get; set; }

        [BsonElement("motherId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MotherId { get; set; }

        [BsonElement("fatherId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FatherId { get; set; }

        [BsonElement("childrenIds")]
        public List<string> ChildrenIds { get; set; } = new();
        
        [BsonElement("employeesAssigned")]
        public List<string> EmployeesAssigned { get; set; } = new();

        // 🟢 Зручне відображення
        [BsonIgnore]
        public string DisplayName
        {
            get
            {
                string smallId = !string.IsNullOrEmpty(Id) && Id.Length >= 6 ? Id.Substring(0, 6) : "new";
                string year = BirthDate != DateTime.MinValue ? BirthDate.Year.ToString() : "unknown";
                return $"[{smallId}] {Name} ({Species}, {Gender}, {year})";
            }
        }
    }
}
