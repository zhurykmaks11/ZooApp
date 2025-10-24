using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models;

public class Exchange
{
    [BsonId]
    public ObjectId Id { get; set; }
    public ObjectId AnimalId { get; set; }
    public string ZooName { get; set; }
    public DateTime ExchangeDate { get; set; }
    public string Type { get; set; } // exchange/gift
}