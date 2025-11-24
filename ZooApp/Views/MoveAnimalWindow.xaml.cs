using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class MoveAnimalWindow : Window
    {
        private readonly CagesService _service;
        private readonly MongoDbContext _context;
        private readonly LogService _log;
        private readonly string _username;

        public MoveAnimalWindow(string username)
        {
            InitializeComponent();

            _username = username;

            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _service = new CagesService(_context);
            _log = new LogService(_context);

            LoadData();
        }

        private void LoadData()
        {
            var animals = _context.Animals.Find(a => a.CageId != null).ToList();
            AnimalBox.ItemsSource = animals;
            AnimalBox.DisplayMemberPath = "DisplayName";
            AnimalBox.SelectedValuePath = "Id";

            var cages = _service.GetAllCages();
            CageBox.ItemsSource = cages;
            CageBox.DisplayMemberPath = "Location";
            CageBox.SelectedValuePath = "Id";
        }

        private void Move_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalBox.SelectedValue == null || CageBox.SelectedValue == null)
            {
                MessageBox.Show("Select animal and cage!");
                return;
            }

            bool ok = _service.MoveAnimal(
                AnimalBox.SelectedValue.ToString(),
                CageBox.SelectedValue.ToString()
            );

            if (!ok)
            {
                MessageBox.Show("❌ Move failed");
                return;
            }

            var animal = AnimalBox.SelectedItem as Animal;
            var cage = CageBox.SelectedItem;
            string cageLocation = cage?.GetType().GetProperty("Location")?.GetValue(cage)?.ToString() ?? "?";

            _log.Write(_username, "Move Animal",
                $"Animal={animal?.Name}, NewCage={cageLocation}");

            MessageBox.Show("✅ Animal moved!");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
