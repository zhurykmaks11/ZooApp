using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models;

public class Cage
{
    [BsonId]
    public ObjectId Id { get; set; }
    public int Number { get; set; }
    public string Location { get; set; }
    public string Size { get; set; }
    public bool Heated { get; set; }
    public List<string> CompatibleSpecies { get; set; }
}