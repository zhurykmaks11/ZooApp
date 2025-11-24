using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class MedicalWindow : Window
    {
        private readonly string _role;
        private readonly string _username;

        private readonly MongoDbContext _context;
        private readonly MedicalService _medicalService;
        private readonly IMongoCollection<Animal> _animalsCollection;
        private readonly LogService _log;

        public MedicalWindow(string role, string username)
        {
            InitializeComponent();

            _role = role?.ToLower() ?? "guest";
            _username = username;

            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _medicalService = new MedicalService(_context);
            _animalsCollection = _context.Animals;
            _log = new LogService(_context);

            LoadRecords();
            ApplyAccessRules();
        }

        // 🧩 Завантаження таблиці
        private void LoadRecords()
        {
            var animals = _animalsCollection.Find(_ => true).ToList();
            var records = _medicalService.GetAllRecords();

            var data = records.Select(r =>
            {
                var animal = animals.FirstOrDefault(a =>
                    r.AnimalId != null && a.Id == r.AnimalId);

                var last = r.Checkups
                    .OrderByDescending(c => c.Date)
                    .FirstOrDefault();

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
            switch (_role)
            {
                case "admin":
                    // full access
                    break;

                case "operator":
                    AddRecordButton.IsEnabled = false;
                    AddCheckupButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    // редагувати останній чекап теж логічно заборонити
                    EditCheckupButton.IsEnabled = false;
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

        // ➕ Новий запис (або перший чекап)
        private void AddRecord_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddMedicalWindow(); // вибір тварини всередині
            if (dialog.ShowDialog() == true)
            {
                LoadRecords();
                _log.Write(_username, "Add Medical Record", "Created or updated medical record");
            }
        }
        private void History_Click(object sender, RoutedEventArgs e)
        {
            int index = MedicalGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("Select a record first.");
                return;
            }

            var records = _medicalService.GetAllRecords();
            var record = records.ElementAt(index);

            var win = new MedicalHistoryWindow(record.Id.ToString());
            win.ShowDialog();
        }

        // 💉 Додати Checkup до вже існуючої картки
        private void AddCheckup_Click(object sender, RoutedEventArgs e)
        {
            int index = MedicalGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("Select a record first.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var records = _medicalService.GetAllRecords();
            var record = records.ElementAt(index);

            var animal = _animalsCollection.Find(a => a.Id == record.AnimalId).FirstOrDefault();
            if (animal == null)
            {
                MessageBox.Show("Animal not found for this record.");
                return;
            }

            var dialog = new AddMedicalWindow(animal); // зафіксована тварина
            if (dialog.ShowDialog() == true)
            {
                LoadRecords();
                _log.Write(_username, "Add Checkup", $"Checkup added to animal {animal.Name}");
            }
        }

        // ✏️ Редагувати останній checkup
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

            var lastCheckup = record.Checkups
                .OrderByDescending(c => c.Date)
                .FirstOrDefault();

            if (lastCheckup == null)
            {
                MessageBox.Show("This record has no checkups to edit.");
                return;
            }

            var dialog = new EditMedicalWindow(lastCheckup);
            if (dialog.ShowDialog() == true)
            {
                _medicalService.UpdateLatestCheckup(record.Id, dialog.UpdatedCheckup);
                LoadRecords();

                _log.Write(_username, "Edit Checkup", $"Updated checkup for record {record.Id}");
            }
        }

        // ❌ Видалити картку
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = MedicalGrid.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("Select a record first.");
                return;
            }

            var records = _medicalService.GetAllRecords();
            var record = records.ElementAt(index);

            if (MessageBox.Show("Delete this medical record?", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            _medicalService.DeleteRecord(record.Id);
            LoadRecords();

            _log.Write(_username, "Delete Medical Record", $"RecordId={record.Id}");
        }

        // 🔍 Пошук
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();

            var animals = _animalsCollection.Find(_ => true).ToList();
            var records = string.IsNullOrEmpty(query) || query == "search..."
                ? _medicalService.GetAllRecords()
                : _medicalService.SearchByDiseaseOrVaccine(query);

            var view = records.Select(r =>
            {
                var animal = animals.FirstOrDefault(a => a.Id == r.AnimalId);
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
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search...";
                SearchBox.Foreground = Brushes.Gray;
            }
        }
    }
}
