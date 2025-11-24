using System;
using System.Linq;
using System.Windows;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class EditMedicalWindow : Window
    {
        public Checkup UpdatedCheckup { get; private set; }

        private readonly Checkup _original;

        public EditMedicalWindow(Checkup checkup)
        {
            InitializeComponent();

            _original = checkup ?? throw new ArgumentNullException(nameof(checkup));

            DatePicker.SelectedDate = checkup.Date;
            WeightBox.Text = checkup.Weight.ToString();
            HeightBox.Text = checkup.Height.ToString();
            VaccinationsBox.Text = string.Join(", ", checkup.Vaccinations ?? Enumerable.Empty<string>());
            IllnessesBox.Text = string.Join(", ", checkup.Illnesses ?? Enumerable.Empty<string>());
            TreatmentBox.Text = checkup.Treatment;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            UpdatedCheckup = new Checkup
            {
                Date = DatePicker.SelectedDate ?? _original.Date,
                Weight = double.TryParse(WeightBox.Text, out var w) ? w : _original.Weight,
                Height = double.TryParse(HeightBox.Text, out var h) ? h : _original.Height,
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