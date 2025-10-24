using MongoDB.Bson;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class EmployeeService
    {
        private readonly IMongoCollection<Employee> _employees;

        public EmployeeService(MongoDbContext context)
        {
            _employees = context.Employees;
        }

        // ➕ Додати працівника
        public void AddEmployee(Employee employee)
        {
            _employees.InsertOne(employee);
        }

        // 🔍 Отримати працівника по імені
        public Employee GetByName(string fullName)
        {
            return _employees.Find(e => e.FullName == fullName).FirstOrDefault();
        }

        // 📃 Усі працівники
        public List<Employee> GetAll()
        {
            return _employees.Find(_ => true).ToList();
        }

        // 📌 Призначити тварину під опіку
        public void AssignAnimal(ObjectId employeeId, ObjectId animalId)
        {
            var update = Builders<Employee>.Update.AddToSet(e => e.AnimalsUnderCare, animalId);
            _employees.UpdateOne(e => e.Id == employeeId, update);
        }
    }
}