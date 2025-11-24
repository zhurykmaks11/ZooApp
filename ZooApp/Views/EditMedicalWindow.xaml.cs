using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class EditMedicalWindow : Window
    {
        public Checkup UpdatedCheckup { get; private set; }

        public EditMedicalWindow(Checkup checkup)
        {
            InitializeComponent();

            // заповнюємо початкові значення
            DatePicker.SelectedDate = checkup.Date;
            WeightBox.Text = checkup.Weight.ToString();
            HeightBox.Text = checkup.Height.ToString();
            VaccinationsBox.Text = string.Join(", ", checkup.Vaccinations);
            IllnessesBox.Text = string.Join(", ", checkup.Illnesses);
            TreatmentBox.Text = checkup.Treatment;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валідація
            if (!double.TryParse(WeightBox.Text, out var w) || w <= 0)
            {
                MessageBox.Show("Weight must be a positive number.");
                return;
            }

            if (!double.TryParse(HeightBox.Text, out var h) || h <= 0)
            {
                MessageBox.Show("Height must be a positive number.");
                return;
            }

            UpdatedCheckup = new Checkup
            {
                Date = DatePicker.SelectedDate ?? DateTime.Now,
                Weight = w,
                Height = h,
                Vaccinations = VaccinationsBox.Text
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .ToList(),
                Illnesses = IllnessesBox.Text
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Trim())
                    .ToList(),
                Treatment = TreatmentBox.Text.Trim()
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
