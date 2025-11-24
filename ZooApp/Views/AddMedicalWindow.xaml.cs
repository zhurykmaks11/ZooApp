using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class AddMedicalWindow : Window
    {
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Animal> _animalsCollection;
        private readonly MedicalService _medicalService;

        public AddMedicalWindow()
        {
            InitializeComponent();

            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _animalsCollection = _context.Animals;
            _medicalService = new MedicalService(_context);

            LoadAnimals();
            DatePicker.SelectedDate = DateTime.Now;
        }

        // режим, коли тварина вже відома (для AddCheckup)
        public AddMedicalWindow(Animal fixedAnimal) : this()
        {
            if (fixedAnimal != null)
            {
                AnimalComboBox.SelectedValue = fixedAnimal.Id;
                AnimalComboBox.IsEnabled = false;
            }
        }

        private void LoadAnimals()
        {
            var animals = _animalsCollection.Find(_ => true).ToList();

            AnimalComboBox.ItemsSource = animals;
            AnimalComboBox.DisplayMemberPath = "DisplayName";
            AnimalComboBox.SelectedValuePath = "Id";

            if (animals.Any())
                AnimalComboBox.SelectedIndex = 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalComboBox.SelectedItem is not Animal selectedAnimal)
            {
                MessageBox.Show("⚠ Please select an animal before saving.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var checkup = new Checkup
            {
                Date = DatePicker.SelectedDate ?? DateTime.Now,
                Weight = double.TryParse(WeightBox.Text, out var w) ? w : 0,
                Height = double.TryParse(HeightBox.Text, out var h) ? h : 0,
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

            // шукаємо існуючу картку
            var existingRecord = _medicalService
                .GetAllRecords()
                .FirstOrDefault(r => r.AnimalId == selectedAnimal.Id);

            if (existingRecord != null)
            {
                // додаємо новий чекап
                _medicalService.AddCheckup(existingRecord.Id, checkup);
                MessageBox.Show($"✅ Added new checkup for '{selectedAnimal.Name}'.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // створюємо нову картку
                var newRecord = new MedicalRecord
                {
                    AnimalId = selectedAnimal.Id,
                    Checkups = new List<Checkup> { checkup }
                };

                _medicalService.AddMedicalRecord(newRecord);
                MessageBox.Show($"🩺 Created new record for '{selectedAnimal.Name}'.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DialogResult = true;
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
