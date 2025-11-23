using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ZooApp.Models
{
    public class LogRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }

        [BsonElement("user")]
        public string User { get; set; }

        [BsonElement("action")]
        public string Action { get; set; }

        [BsonElement("details")]
        public string Details { get; set; }
    }
}