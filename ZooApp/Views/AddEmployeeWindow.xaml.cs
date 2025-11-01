using System;
using System.Windows;
using System.Windows.Controls;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;
namespace ZooApp.Views
{
    public partial class AddEmployeeWindow : Window
    {
        private readonly EmployeeService _employeeService;

        public AddEmployeeWindow()
        {
            InitializeComponent();

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _employeeService = new EmployeeService(context);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            // 🔴 Full Name
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                NameBox.BorderBrush = System.Windows.Media.Brushes.Red;
                NameError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                NameBox.ClearValue(Border.BorderBrushProperty);
                NameError.Visibility = Visibility.Collapsed;
            }

            // 🔴 Category
            if (string.IsNullOrWhiteSpace(CategoryBox.Text))
            {
                CategoryBox.BorderBrush = System.Windows.Media.Brushes.Red;
                CategoryError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                CategoryBox.ClearValue(Border.BorderBrushProperty);
                CategoryError.Visibility = Visibility.Collapsed;
            }

            // 🔴 Gender
            if (GenderComboBox.SelectedItem == null)
            {
                GenderComboBox.BorderBrush = System.Windows.Media.Brushes.Red;
                GenderError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                GenderComboBox.ClearValue(Border.BorderBrushProperty);
                GenderError.Visibility = Visibility.Collapsed;
            }

            // 🔴 Birth date
            if (BirthDatePicker.SelectedDate == null)
            {
                BirthDatePicker.BorderBrush = System.Windows.Media.Brushes.Red;
                BirthError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                BirthDatePicker.ClearValue(Border.BorderBrushProperty);
                BirthError.Visibility = Visibility.Collapsed;
            }

            // 🔴 Work start date
            if (WorkStartDatePicker.SelectedDate == null)
            {
                WorkStartDatePicker.BorderBrush = System.Windows.Media.Brushes.Red;
                WorkDateError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                WorkStartDatePicker.ClearValue(Border.BorderBrushProperty);
                WorkDateError.Visibility = Visibility.Collapsed;
            }

            // 🔴 Salary
            if (!double.TryParse(SalaryBox.Text, out double salary) || salary <= 0)
            {
                SalaryBox.BorderBrush = System.Windows.Media.Brushes.Red;
                SalaryError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                SalaryBox.ClearValue(Border.BorderBrushProperty);
                SalaryError.Visibility = Visibility.Collapsed;
            }

            if (!isValid) return;

            // ✅ Якщо все валідно — зберігаємо
            var emp = new Employee
            {
                FullName = NameBox.Text.Trim(),
                Category = CategoryBox.Text.Trim(),
                Gender = ((System.Windows.Controls.ComboBoxItem)GenderComboBox.SelectedItem).Content.ToString(),
                BirthDate = BirthDatePicker.SelectedDate.Value,
                WorkStartDate = WorkStartDatePicker.SelectedDate.Value,
                Salary = salary
            };

            _employeeService.AddEmployee(emp);
            MessageBox.Show($"✅ Employee '{emp.FullName}' added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
