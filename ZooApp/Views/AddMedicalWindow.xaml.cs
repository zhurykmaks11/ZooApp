using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MongoDB.Bson;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class AddMedicalWindow : Window
    {
        public Checkup Checkup { get; private set; }
        public string SelectedAnimalId { get; private set; }  // ❗ змінено на string

        private readonly IMongoCollection<Animal> _animalsCollection;

        public AddMedicalWindow()
        {
            InitializeComponent();

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _animalsCollection = context.Animals;

            LoadAnimals();
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void LoadAnimals()
        {
            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            var animals = context.Animals.Find(_ => true).ToList();

            AnimalComboBox.ItemsSource = animals;
            AnimalComboBox.DisplayMemberPath = "DisplayName";
            AnimalComboBox.SelectedValuePath = "Id";
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
                Vaccinations = VaccinationsBox.Text.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim()).ToList(),
                Illnesses = IllnessesBox.Text.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList(),
                Treatment = TreatmentBox.Text.Trim()
            };

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            var service = new MedicalService(context);

            // 🩺 Знаходимо картку тварини
            var existingRecord = service.GetAllRecords()
                .FirstOrDefault(r => r.AnimalId == selectedAnimal.Id.ToString()); // ✅ string == string

            if (existingRecord != null)
            {
                service.AddCheckup(existingRecord.Id.ToString(), checkup);
                MessageBox.Show($"✅ Added new checkup for '{selectedAnimal.Name}'.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var newRecord = new MedicalRecord
                {
                    AnimalId = selectedAnimal.Id.ToString(), // ✅ у MedicalRecord тепер string
                    Checkups = new List<Checkup> { checkup }
                };

                service.AddMedicalRecord(newRecord);
                MessageBox.Show($"🩺 Created new record for '{selectedAnimal.Name}'.", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DialogResult = true;
            Close();
        }

        private void ClearFieldsAfterSave()
        {
            DatePicker.SelectedDate = DateTime.Now;
            WeightBox.Clear();
            HeightBox.Clear();
            VaccinationsBox.Clear();
            IllnessesBox.Clear();
            TreatmentBox.Clear();
            WeightBox.Focus();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
