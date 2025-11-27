using System.Linq;
using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class CageWindow : Window
    {
        private readonly CagesService _cages;
        private readonly IMongoCollection<Animal> _animals;
        private readonly LogService _log;

        private readonly string _role;
        private readonly string _username;

        public CageWindow(string role, string username)
        {
            InitializeComponent();

            _role = role.ToLower();
            _username = username;

            var ctx = new MongoDbContext("mongodb://localhost:27017", "test");

            _cages = new CagesService(ctx);
            _animals = ctx.Animals;
            _log = new LogService(ctx);

            ApplyPermissions();
            LoadCages();
        }

        private void ApplyPermissions()
        {
            if (_role is "admin" or "operator") return;

            AddCageButton.IsEnabled = false;
            EditCageButton.IsEnabled = false;
            DeleteCageButton.IsEnabled = false;
            AssignAnimalButton.IsEnabled = false;
            MoveAnimalButton.IsEnabled = false;
        }

        private void LoadCages()
        {
            var list = _cages.GetAllCages()
                .Select(c => new
                {
                    c.Id,
                    c.Location,
                    c.Size,
                    Heated = c.Heated ? "Yes" : "No",
                    c.Capacity,
                    AnimalCount = _animals.CountDocuments(a => a.CageId == c.IdString)
                })
                .ToList();

            CagesGrid.ItemsSource = list;
        }

        private void AssignAnimal_Click(object sender, RoutedEventArgs e)
        {
            var win = new AssignAnimalToCageWindow(_username);
            if (win.ShowDialog() == true) LoadCages();
        }

        private void MoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            var win = new MoveAnimalWindow(_username);
            if (win.ShowDialog() == true) LoadCages();
        }

        private void ViewAnimals_Click(object sender, RoutedEventArgs e)
        {
            if (CagesGrid.SelectedItem == null)
            {
                MessageBox.Show("Select cage");
                return;
            }

            var cageId = ((dynamic)CagesGrid.SelectedItem).Id.ToString();
            new ViewCageAnimalsWindow(cageId).ShowDialog();
        }

        private void AddCage_Click(object sender, RoutedEventArgs e)
        {
            var w = new AddCageWindow(_username);
            if (w.ShowDialog() == true) LoadCages();
        }

        private void EditCage_Click(object sender, RoutedEventArgs e)
        {
            if (CagesGrid.SelectedItem == null)
                return;

            string id = ((dynamic)CagesGrid.SelectedItem).Id.ToString();
            var w = new EditCageWindow(id, _username);
            if (w.ShowDialog() == true) LoadCages();
        }

        private void DeleteCage_Click(object sender, RoutedEventArgs e)
        {
            if (CagesGrid.SelectedItem == null) return;
            string id = ((dynamic)CagesGrid.SelectedItem).Id.ToString();

            if (_animals.CountDocuments(a => a.CageId == id) > 0)
            {
                MessageBox.Show("❌ Cannot delete cage with animals");
                return;
            }

            if (MessageBox.Show("Delete cage?", "Confirm", MessageBoxButton.YesNo)
                == MessageBoxResult.Yes)
            {
                var cage = _cages.GetCage(id);
                _cages.DeleteCage(id);

                _log.Write(_username, "Delete Cage", cage.Location);
                LoadCages();
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }
    }
}
