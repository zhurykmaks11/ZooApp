using System.Linq;
using System.Windows;
using System.Windows.Media;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class AnimalsWindow : Window
    {
        private readonly IMongoCollection<Animal> _animals;
        private readonly string _role;
        private readonly string _username;

        private readonly LogService _log;

        public AnimalsWindow(string role, string username)
        {
            InitializeComponent();

            _role = role.ToLower();
            _username = username;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");

            _animals = context.Animals;
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
                    break; // full access

                case "operator":
                    break; // can add/edit/delete

                case "authorized":
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;

                case "guest":
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;
            }
        }

        private void LoadAnimals()
        {
            AnimalsGrid.ItemsSource = _animals.Find(_ => true).ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditAnimalWindow(_role);
            if (window.ShowDialog() == true)
            {
                window.Animal.Id = null;

                _animals.InsertOne(window.Animal);

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

            var window = new AddEditAnimalWindow(_role, selected);

            if (window.ShowDialog() == true)
            {
                _animals.ReplaceOne(a => a.Id == selected.Id, window.Animal);

                _log.Write(_username, "Edit Animal",
                    $"Edited animal: {window.Animal.Name}");

                LoadAnimals();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is not Animal selected)
            {
                MessageBox.Show("Select an animal.");
                return;
            }

            if (MessageBox.Show($"Delete {selected.Name}?", "Confirm",
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _animals.DeleteOne(a => a.Id == selected.Id);

                _log.Write(_username, "Delete Animal",
                    $"Deleted animal: {selected.Name}");

                LoadAnimals();
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string q = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(q))
            {
                LoadAnimals();
                return;
            }

            AnimalsGrid.ItemsSource = _animals.Find(a =>
                a.Name.ToLower().Contains(q) ||
                a.Species.ToLower().Contains(q)
            ).ToList();
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
