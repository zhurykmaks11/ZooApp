using System;
using System.Windows;
using System.Windows.Controls;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class EditEmployeeWindow : Window
    {
        private readonly EmployeeService _employeeService;
        private readonly Employee _employee;

        public EditEmployeeWindow(Employee employee, EmployeeService service)
        {
            InitializeComponent();
            _employeeService = service;
            _employee = employee;

            // Заповнюємо поля поточними даними
            NameBox.Text = employee.FullName;
            CategoryBox.Text = employee.Category;
            GenderComboBox.SelectedItem = employee.Gender == "male"
                ? GenderComboBox.Items[0]
                : GenderComboBox.Items[1];
            BirthDatePicker.SelectedDate = employee.BirthDate;
            WorkStartDatePicker.SelectedDate = employee.WorkStartDate;
            SalaryBox.Text = employee.Salary.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка валідації
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(CategoryBox.Text))
            {
                MessageBox.Show("Category cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (GenderComboBox.SelectedItem is not ComboBoxItem genderItem)
            {
                MessageBox.Show("Please select a gender.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(SalaryBox.Text, out var salary) || salary <= 0)
            {
                MessageBox.Show("Salary must be a positive number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Оновлення даних
            _employee.FullName = NameBox.Text.Trim();
            _employee.Category = CategoryBox.Text.Trim();
            _employee.Gender = genderItem.Content.ToString();
            _employee.BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now;
            _employee.WorkStartDate = WorkStartDatePicker.SelectedDate ?? DateTime.Now;
            _employee.Salary = salary;

            _employeeService.UpdateEmployee(_employee);

            MessageBox.Show("✅ Employee updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
