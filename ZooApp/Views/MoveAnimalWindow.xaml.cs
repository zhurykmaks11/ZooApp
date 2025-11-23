using System.Linq;
using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class MoveAnimalWindow : Window
    {
        private readonly CagesService _service;
        private readonly MongoDbContext _context;

        public MoveAnimalWindow()
        {
            InitializeComponent();
            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _service = new CagesService(_context);

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

            MessageBox.Show(ok ? "✅ Animal moved!" : "❌ Move failed");

            if (ok)
            {
                DialogResult = true;
                Close(); // додати!
            }

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}