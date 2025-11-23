using System;
using System.Linq;
using System.Text;
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
        
        public EmployeeWindow(string role)
        {
            InitializeComponent();
            _role = role;

            var context = new Data.MongoDbContext("mongodb://localhost:27017", "test");
            _employeeService = new EmployeeService(context);

            LoadEmployees();
            ApplyAccessRules();
        }

        // 🔒 Обмеження доступу за ролями
        private void ApplyAccessRules()
        {
            switch (_role.ToLower())
            {
                case "admin":
                    // повний доступ
                    break;

                case "operator":
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;

                case "guest":
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;
            }
        }

        // 📋 Завантаження таблиці
        private void LoadEmployees()
        {
            EmployeeGrid.ItemsSource = _employeeService.GetAllEmployees();
        }

        // ➕ Додати
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEmployeeWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        // ✏️ Редагувати
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItem is not Employee selected)
            {
                MessageBox.Show("Select an employee to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var editWindow = new EditEmployeeWindow(selected, _employeeService);
            if (editWindow.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        // ❌ Видалити
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItem is not Employee selected)
            {
                MessageBox.Show("Select an employee to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Delete {selected.FullName}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _employeeService.DeleteEmployee(selected.Id);
                LoadEmployees();
            }
        }

        // ⬅ Назад
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            this.Close();
        }

        // 🔍 Фільтр
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

        // 🧠 Placeholder логіка
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
        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            var empWindow = new EmployeeWindow(_role);
            empWindow.Show();
            this.Close();
        }

    }
}
