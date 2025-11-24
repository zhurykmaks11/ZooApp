using System.Linq;
using System.Windows;
using System.Windows.Media;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class AnimalsWindow : Window
    {
        private readonly AnimalsService _animalsService;
        private readonly string _role;
        private readonly string _username;

        private readonly LogService _log;

        public AnimalsWindow(string role, string username)
        {
            InitializeComponent();

            _role = role.ToLower();
            _username = username;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");

            _animalsService = new AnimalsService(context);
            _log = new LogService(context);

            LoadAnimals();
            ApplyRoleRules();
        }

        // 🔐 RULES FOR ROLES
        private void ApplyRoleRules()
        {
            switch (_role)
            {
                case "admin":
                    // full access
                    break;

                case "operator":
                    // теж має повний CRUD по тваринах
                    break;

                case "authorized":
                case "guest":
                    AssignEmployees.IsEnabled = false;
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;
            }
        }

        private void LoadAnimals()
        {
            AnimalsGrid.ItemsSource = _animalsService.GetAllAnimals();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditAnimalWindow(_role);
            if (window.ShowDialog() == true)
            {
                // при InsertOne драйвер Mongo заповнить Id
                _animalsService.AddAnimal(window.Animal);

                // якщо є батьки — прив'язуємо дитину
                if (!string.IsNullOrEmpty(window.Animal.Id))
                {
                    _animalsService.AddChild(
                        window.Animal.MotherId,
                        window.Animal.FatherId,
                        window.Animal.Id);
                }

                _log.Write(_username, "Add Animal",
                    $"Added new animal: {window.Animal.Name}");

                LoadAnimals();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is not Animal selected)
            {
                MessageBox.Show("Select an animal.");
                return;
            }

            // Створюємо копію, щоб не правити напряму в гріді
            var clone = new Animal
            {
                Id = selected.Id,
                Name = selected.Name,
                Species = selected.Species,
                Gender = selected.Gender,
                BirthDate = selected.BirthDate,
                Weight = selected.Weight,
                Height = selected.Height,
                Type = selected.Type,
                ClimateZone = selected.ClimateZone,
                CageId = selected.CageId,
                FeedingScheduleId = selected.FeedingScheduleId,
                MedicalRecordId = selected.MedicalRecordId,
                NeedsWarmShelter = selected.NeedsWarmShelter,
                IsIsolated = selected.IsIsolated,
                IsolationReason = selected.IsolationReason,
                MotherId = selected.MotherId,
                FatherId = selected.FatherId,
                ChildrenIds = selected.ChildrenIds?.ToList() ?? new()
            };

            var window = new AddEditAnimalWindow(_role, clone);

            if (window.ShowDialog() == true)
            {
                _animalsService.UpdateAnimal(window.Animal);

                _log.Write(_username, "Edit Animal",
                    $"Edited animal: {window.Animal.Name}");

                LoadAnimals();
            }
        }
        private void AssignEmployees_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is not Animal selected)
            {
                MessageBox.Show("Select an animal.");
                return;
            }

            var win = new AssignEmployeesWindow(selected);
            if (win.ShowDialog() == true)
                LoadAnimals();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is not Animal selected)
            {
                MessageBox.Show("Select an animal.");
                return;
            }

            if (MessageBox.Show($"Delete {selected.Name}?", "Confirm",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _animalsService.DeleteAnimal(selected.Id);

                _log.Write(_username, "Delete Animal",
                    $"Deleted animal: {selected.Name}");

                LoadAnimals();
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string q = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(q) || q == "search by name...")
            {
                LoadAnimals();
                return;
            }

            var all = _animalsService.GetAllAnimals();

            AnimalsGrid.ItemsSource = all
                .Where(a =>
                    a.Name.ToLower().Contains(q) ||
                    a.Species.ToLower().Contains(q))
                .ToList();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search by name...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search by name...";
                SearchBox.Foreground = Brushes.Gray;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }
    }
}
