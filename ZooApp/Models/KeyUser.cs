using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models
{
    public class KeyUser
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("login")]
        public string Login { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("role")]
        public string Role { get; set; }
    }
}