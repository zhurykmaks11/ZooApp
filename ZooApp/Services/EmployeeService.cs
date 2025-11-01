using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
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

        public List<Employee> GetAllEmployees() =>
            _employees.Find(_ => true).ToList();

        public void AddEmployee(Employee emp) =>
            _employees.InsertOne(emp);

        public void DeleteEmployee(string id) =>
            _employees.DeleteOne(e => e.Id == id);

        public List<Employee> Search(string keyword)
        {
            keyword = keyword.ToLower();
            return _employees.Find(e =>
                e.FullName.ToLower().Contains(keyword) ||
                e.Category.ToLower().Contains(keyword) ||
                e.Gender.ToLower().Contains(keyword)
            ).ToList();
        }

        public List<Employee> FilterByRole(string role) =>
            _employees.Find(e => e.Category.ToLower() == role.ToLower()).ToList();
    }
}