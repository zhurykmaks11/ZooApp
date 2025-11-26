using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models;

public class Feed
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } 
    public bool ProducedByZoo { get; set; }
    public List<ObjectId> Suppliers { get; set; }
}