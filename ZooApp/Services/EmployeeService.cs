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

        public void AddEmployee(Employee employee) =>
            _employees.InsertOne(employee);
        
        
        public void UpdateEmployee(Employee employee)
        {
            var filter = Builders<Employee>.Filter.Eq(e => e.Id, employee.Id);
            _employees.ReplaceOne(filter, employee);
        }
        public void DeleteEmployee(string id)
        {
            var filter = Builders<Employee>.Filter.Eq(e => e.Id, id);
            _employees.DeleteOne(filter);
        }
        
        public List<Employee> SearchEmployees(string query)
        {
            query = query.ToLower();
            return _employees.Find(e =>
                e.FullName.ToLower().Contains(query) ||
                e.Category.ToLower().Contains(query)).ToList();
        }

        public List<Employee> FilterByRole(string role) =>
            _employees.Find(e => e.Category.ToLower() == role.ToLower()).ToList();
    }
}