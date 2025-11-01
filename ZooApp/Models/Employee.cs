using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace ZooApp.Models
{
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("fullName")]
        public string FullName { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } // ветеринар, прибиральник, дресирувальник...

        [BsonElement("gender")]
        public string Gender { get; set; }

        [BsonElement("birthDate")]
        public DateTime BirthDate { get; set; }

        [BsonElement("workStartDate")]
        public DateTime WorkStartDate { get; set; }

        [BsonElement("salary")]
        public double Salary { get; set; }

        [BsonElement("animalsUnderCare")]
        public List<string> AnimalsUnderCare { get; set; } = new();
    }
}