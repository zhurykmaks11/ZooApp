using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ZooApp.Models
{
    public class Supplier
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("feedTypes")]
        public List<string> FeedTypes { get; set; } = new();

        [BsonElement("contracts")]
        public List<string> Contracts { get; set; } = new();

        
        [BsonIgnore]
        public string FeedTypesString => string.Join(", ", FeedTypes);
    }
}