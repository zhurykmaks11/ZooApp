using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZooApp.Models;

public class Employee
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string FullName { get; set; }
    public string Category { get; set; } // ветеринар, дресирувальник, прибиральник тощо
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; }
    public double Salary { get; set; }
    public DateTime WorkStartDate { get; set; }

    public List<ObjectId> AnimalsUnderCare { get; set; }
}