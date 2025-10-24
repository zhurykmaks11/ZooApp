using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models;
public class FeedingSchedule
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId AnimalId { get; set; }
    public string Season { get; set; }
    public List<Meal> Meals { get; set; }
}

public class Meal
{
    public string Time { get; set; }
    public ObjectId FeedId { get; set; }
    public double AmountKg { get; set; }
}