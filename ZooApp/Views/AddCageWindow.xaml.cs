using MongoDB.Bson;
using System;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly LogService _log;
        private readonly string _username;

        private readonly Regex sizeRegex =
            new Regex(@"^\s*\d{1,3}\s*[xX]\s*\d{1,3}\s*$");

        public AddCageWindow(string username)
        {
            InitializeComponent();

            _username = username;

            // ✔ ЄДИНИЙ КОНТЕКСТ
            var context = new MongoDbContext("mongodb://localhost:27017", "test");

            // ✔ СЕРВІСИ
            _cageService = new CagesService(context);
            _log = new LogService(context);
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

            // VALIDATE SIZE
            if (!sizeRegex.IsMatch(SizeBox.Text))
            {
                MessageBox.Show("❌ Size must be in format NNxNN (example: 50x30)", "Error");
                return;
            }

            if (!int.TryParse(CapacityBox.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Capacity must be a positive number!");
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

            // ✔ ЛОГУВАННЯ
            _log.Write(_username, "Add Cage", $"Location={cage.Location}, Size={cage.Size}, Capacity={cage.Capacity}");

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
