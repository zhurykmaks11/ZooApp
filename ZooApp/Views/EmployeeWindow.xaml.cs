using System;
using System.Linq;
using System.Windows;
using ZooApp.Services;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class EmployeeWindow : Window
    {
        private readonly string _role;
        private readonly EmployeeService _employeeService;
        private readonly string _username;
        private readonly LogService _log;

        public EmployeeWindow(string role, string username)
        {
            InitializeComponent();
            _role = role;
            _username = username;

            var context = new Data.MongoDbContext("mongodb://localhost:27017", "test");
            _employeeService = new EmployeeService(context);
            _log = new LogService(context);

            LoadEmployees();
            ApplyAccessRules();
        }

        private void ApplyAccessRules()
        {
            switch (_role.ToLower())
            {
                case "admin":
                    break;

                case "operator":
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;

                case "authorized":
                case "guest":
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;
            }
        }

        private void LoadEmployees()
        {
            EmployeeGrid.ItemsSource = _employeeService.GetAllEmployees();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEmployeeWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadEmployees();
                _log.Write(_username, "Add Employee", "New employee created");
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItem is not Employee selected)
            {
                MessageBox.Show("Select an employee to edit.");
                return;
            }

            var editWindow = new EditEmployeeWindow(selected, _employeeService);
            if (editWindow.ShowDialog() == true)
            {
                LoadEmployees();
                _log.Write(_username, "Edit Employee", $"Updated EmployeeId={selected.Id}");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItem is not Employee selected)
            {
                MessageBox.Show("Select an employee to delete.");
                return;
            }

            if (MessageBox.Show($"Delete {selected.FullName}?",
                    "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _employeeService.DeleteEmployee(selected.Id);
                LoadEmployees();
                _log.Write(_username, "Delete Employee", $"EmployeeId={selected.Id}");
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();
            string category = (CategoryFilter.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "All";
            string gender = (GenderFilter.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString() ?? "All";

            var employees = _employeeService.GetAllEmployees();

            if (!string.IsNullOrWhiteSpace(query) && query != "search by name...")
                employees = employees.Where(e => e.FullName.ToLower().Contains(query)).ToList();

            if (category != "All")
                employees = employees.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

            if (gender != "All")
                employees = employees.Where(e => e.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase)).ToList();

            EmployeeGrid.ItemsSource = employees;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search by name...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search by name...";
                SearchBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }
    }
}
