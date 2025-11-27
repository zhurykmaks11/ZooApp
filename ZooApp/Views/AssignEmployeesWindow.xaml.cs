using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class AssignEmployeesWindow : Window
    {
        private readonly Animal _animal;
        private readonly EmployeeService _employeeService;
        private readonly AnimalsService _animalsService;

        private List<Employee> _allEmployees;
        private List<Employee> _assignedEmployees;

        public AssignEmployeesWindow(Animal animal)
        {
            InitializeComponent();

            _animal = animal;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _employeeService = new EmployeeService(context);
            _animalsService = new AnimalsService(context);

            LoadData();
        }

        private void LoadData()
        {
            _allEmployees = _employeeService.GetAllEmployees();
            
            var allowed = new[] { "vet", "cleaner", "trainer" };

            _allEmployees = _employeeService.GetAllEmployees()
                .Where(e => allowed.Contains(e.Category.ToLower()))
                .ToList();


            _assignedEmployees = _allEmployees
                .Where(e => e.AnimalsUnderCare.Contains(_animal.Id))
                .ToList();

            AllEmployeesList.ItemsSource = _allEmployees.Except(_assignedEmployees).ToList();
            AssignedList.ItemsSource = _assignedEmployees;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (AllEmployeesList.SelectedItem is Employee emp)
            {
                _assignedEmployees.Add(emp);
                AllEmployeesList.ItemsSource = _allEmployees.Except(_assignedEmployees).ToList();
                AssignedList.ItemsSource = _assignedEmployees.ToList();
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (AssignedList.SelectedItem is Employee emp)
            {
                _assignedEmployees.Remove(emp);
                AllEmployeesList.ItemsSource = _allEmployees.Except(_assignedEmployees).ToList();
                AssignedList.ItemsSource = _assignedEmployees.ToList();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (var emp in _allEmployees)
            {
                if (_assignedEmployees.Contains(emp))
                {
                    if (!emp.AnimalsUnderCare.Contains(_animal.Id))
                        emp.AnimalsUnderCare.Add(_animal.Id);
                }
                else
                {
                    emp.AnimalsUnderCare.Remove(_animal.Id);
                }

                _employeeService.UpdateEmployee(emp);
            }
            
            _animal.EmployeesAssigned = _assignedEmployees.Select(a => a.Id).ToList();
            _animalsService.UpdateAnimal(_animal);

            MessageBox.Show("Changes saved!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
