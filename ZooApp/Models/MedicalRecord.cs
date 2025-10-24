using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models;
public class MedicalRecord
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId AnimalId { get; set; }
    public List<Checkup> Checkups { get; set; }
}

public class Checkup
{
    public DateTime Date { get; set; }
    public double Weight { get; set; }
    public double Height { get; set; }
    public List<string> Vaccinations { get; set; }
    public List<string> Illnesses { get; set; }
    public string Treatment { get; set; }
}