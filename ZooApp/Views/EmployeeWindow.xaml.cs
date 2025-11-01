using System;
using System.Linq;
using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class EmployeeWindow : Window
    {
        private readonly EmployeeService _employeeService;
        private readonly string _role;

        public EmployeeWindow(string role)
        {
            InitializeComponent();
            _role = role;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _employeeService = new EmployeeService(context);

            LoadEmployees();
        }

        private void LoadEmployees()
        {
            EmployeeGrid.ItemsSource = _employeeService.GetAllEmployees();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEmployeeWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeGrid.SelectedItem is Employee emp)
            {
                _employeeService.DeleteEmployee(emp.Id);
                LoadEmployees();
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string q = SearchBox.Text.Trim().ToLower();
            var data = string.IsNullOrEmpty(q) ? _employeeService.GetAllEmployees() : _employeeService.Search(q);
            EmployeeGrid.ItemsSource = data;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role).Show();
            Close();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search...";
                SearchBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
