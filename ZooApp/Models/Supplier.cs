using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models;

public class Supplier
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public List<string> FeedTypes { get; set; }
    public List<Contract> Contracts { get; set; }
}

public class Contract
{
    public ObjectId FeedId { get; set; }
    public double Price { get; set; }
    public DateTime Date { get; set; }
}