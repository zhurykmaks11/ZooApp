using MongoDB.Bson;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZooApp.Models;
using ZooApp.Services;
using ZooApp.Data;

namespace ZooApp.Views
{
    public partial class AddCageWindow : Window
    {
        private readonly CagesService _cageService;

        public AddCageWindow()
        {
            InitializeComponent();
            _cageService = new CagesService(new MongoDbContext("mongodb://localhost:27017", "test"));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LocationBox.Text) ||
                string.IsNullOrWhiteSpace(SizeBox.Text) ||
                string.IsNullOrWhiteSpace(CapacityBox.Text) ||
                HeatedBox.SelectedItem == null)
            {
                MessageBox.Show("⚠ Please fill all fields!", "Error");
                return;
            }

            if (!int.TryParse(CapacityBox.Text, out int capacity))
            {
                MessageBox.Show("Capacity must be a number!");
                return;
            }

            string heatedText = (HeatedBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            bool heated = heatedText == "Yes";

            string typeSelected = (TypesBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "both";

            var cage = new Cage
            {
                Id = ObjectId.GenerateNewId(),
                Number = 0,
                Location = LocationBox.Text.Trim(),
                Size = SizeBox.Text.Trim(),
                Heated = heated,
                Capacity = capacity,
                CompatibleSpecies = SpeciesBox.Text
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim().ToLower())
                    .ToList(),
                AllowedTypes = typeSelected switch
                {
                    "herbivore" => new() { "herbivore" },
                    "predator" => new() { "predator" },
                    _ => new() { "herbivore", "predator" }
                }
            };

            _cageService.AddCage(cage);

            MessageBox.Show("✅ Cage added!");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
