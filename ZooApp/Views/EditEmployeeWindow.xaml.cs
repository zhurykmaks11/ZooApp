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

            _employee = employee;
            _employeeService = service;

            LoadData();
        }

        private void LoadData()
        {
            NameBox.Text = _employee.FullName;

            // CATEGORY
            foreach (ComboBoxItem item in CategoryCombo.Items)
            {
                if (item.Content.ToString().ToLower() == _employee.Category.ToLower())
                {
                    CategoryCombo.SelectedItem = item;
                    break;
                }
            }

            // GENDER
            foreach (ComboBoxItem item in GenderComboBox.Items)
            {
                if (item.Content.ToString().ToLower() == _employee.Gender.ToLower())
                {
                    GenderComboBox.SelectedItem = item;
                    break;
                }
            }

            BirthDatePicker.SelectedDate = _employee.BirthDate;
            WorkStartDatePicker.SelectedDate = _employee.WorkStartDate;
            SalaryBox.Text = _employee.Salary.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (CategoryCombo.SelectedItem is not ComboBoxItem catItem ||
                GenderComboBox.SelectedItem is not ComboBoxItem genderItem)
            {
                MessageBox.Show("Please fill all fields!", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(SalaryBox.Text, out double salary) || salary <= 0)
            {
                MessageBox.Show("Salary must be a positive number.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // UPDATE EMPLOYEE
            _employee.FullName = NameBox.Text.Trim();
            _employee.Category = catItem.Content.ToString();
            _employee.Gender = genderItem.Content.ToString();
            _employee.BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now;
            _employee.WorkStartDate = WorkStartDatePicker.SelectedDate ?? DateTime.Now;
            _employee.Salary = salary;

            _employeeService.UpdateEmployee(_employee);

            MessageBox.Show("Employee updated successfully.",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
