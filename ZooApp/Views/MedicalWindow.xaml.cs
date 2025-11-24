using System;
using System.Linq;
using System.Text;
using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;
using MongoDB.Bson;

namespace ZooApp.Views
{
    public partial class MedicalWindow : Window
    {
        private readonly string _role;
        private readonly MedicalService _medicalService;
        private readonly string _username;
        private readonly LogService _log;

        public MedicalWindow(string role, string username)
        {
            InitializeComponent();
            _role = role;
            _username = username;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _medicalService = new MedicalService(context);
            _log = new LogService(context);
            LoadRecords();
            ApplyAccessRules();
        }

        // 🧩 Завантаження таблиці
        private void LoadRecords()
        {
            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            var animals = context.Animals.Find(_ => true).ToList();
            var records = _medicalService.GetAllRecords();

            var data = records.Select(r =>
            {
                var animal = animals.FirstOrDefault(a =>
                    r.AnimalId != null && a.Id == r.AnimalId.ToString());

                var last = r.Checkups.OrderByDescending(c => c.Date).FirstOrDefault();

                return new
                {
                    AnimalName = animal?.Name ?? "Unknown",
                    LastCheckupDate = last?.Date.ToString("dd.MM.yyyy") ?? "—",
                    Weight = last?.Weight ?? 0,
                    Height = last?.Height ?? 0,
                    Vaccinations = last != null && last.Vaccinations.Any()
                        ? string.Join(", ", last.Vaccinations)
                        : "—"
                };
            }).ToList();

            MedicalGrid.ItemsSource = data;
        }

        // 🔒 Обмеження за роллю
        private void ApplyAccessRules()
        {
            switch (_role.ToLower())
            {
                case "admin":
                    break;

                case "operator":
                    AddRecordButton.IsEnabled = false;
                    AddCheckupButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;

                case "authorized":
                case "guest":
                    AddRecordButton.IsEnabled = false;
                    AddCheckupButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    EditCheckupButton.IsEnabled = false;
                    break;
            }
        }


        // ➕ Новий запис (якщо тварини ще немає)
        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddMedicalWindow();
            if (dialog.ShowDialog() == true)
            {
                
                LoadRecords();
                _log.Write(_username, "Add Medical Record", "Created new medical record");
            }
            

        }

        // 💉 Додати Checkup у вже існуючий запис
        private void AddCheckup_Click(object sender, RoutedEventArgs e)
        {
            if (MedicalGrid.SelectedIndex < 0)
            {
                MessageBox.Show("Select a record first.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new AddMedicalWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadRecords();
            }
            _log.Write(_username, "Add Checkup", $"Checkup added to animal record");

        }
        private void EditCheckup_Click(object sender, RoutedEventArgs e)
        {
            int index = MedicalGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("Select a record first.");
                return;
            }

            var records = _medicalService.GetAllRecords();
            var record = records.ElementAt(index);

            var lastCheckup = record.Checkups.OrderByDescending(c => c.Date).FirstOrDefault();
            if (lastCheckup == null)
            {
                MessageBox.Show("This record has no checkups to edit.");
                return;
            }

            var dialog = new EditMedicalWindow(lastCheckup);
            if (dialog.ShowDialog() == true)
            {
                _medicalService.UpdateLatestCheckup(record.Id.ToString(), dialog.UpdatedCheckup);
                LoadRecords();
            }
            _log.Write(_username, "Edit Checkup", $"Updated checkup for record {record.Id}");

        }

        // ❌ Видалити
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var records = _medicalService.GetAllRecords();
            if (!records.Any())
            {
                MessageBox.Show("No records to delete.");
                return;
            }

            var record = records.ElementAtOrDefault(MedicalGrid.SelectedIndex);
            if (record != null)
            {
                _medicalService.DeleteRecord(record.Id.ToString());
                LoadRecords();
            }
            _log.Write(_username, "Delete Medical Record", $"RecordId={record.Id}");

        }

        // 🔍 Пошук
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();
            var records = string.IsNullOrEmpty(query)
                ? _medicalService.GetAllRecords()
                : _medicalService.SearchByDiseaseOrVaccine(query);

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            var animals = context.Animals.Find(_ => true).ToList();

            var view = records.Select(r =>
            {
                var animal = animals.FirstOrDefault(a =>
                    r.AnimalId != null && a.Id == r.AnimalId.ToString());

                var last = r.Checkups.OrderByDescending(c => c.Date).FirstOrDefault();

                return new
                {
                    AnimalName = animal?.Name ?? "Unknown",
                    LastCheckupDate = last?.Date.ToString("dd.MM.yyyy") ?? "—",
                    Weight = last?.Weight ?? 0,
                    Height = last?.Height ?? 0,
                    Vaccinations = last != null && last.Vaccinations.Any()
                        ? string.Join(", ", last.Vaccinations)
                        : "—"
                };
            }).ToList();

            MedicalGrid.ItemsSource = view;
        }

        // 📊 Статистика
        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            var stats = _medicalService.GetVaccinationStatistics();
            if (stats.Count == 0)
            {
                MessageBox.Show("No vaccination data available.");
                return;
            }

            var sb = new StringBuilder("📊 Vaccination Statistics:\n\n");
            foreach (var kvp in stats)
                sb.AppendLine($"{kvp.Key}: {kvp.Value}");

            MessageBox.Show(sb.ToString(), "Stats", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }

        // 🧠 Placeholder логіка
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
