using System.Linq;
using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class AssignAnimalToCageWindow : Window
    {
        private readonly CagesService _cagesService;
        private readonly IMongoCollection<Animal> _animals;

        public AssignAnimalToCageWindow()
        {
            InitializeComponent();

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _cagesService = new CagesService(context);
            _animals = context.Animals;

            LoadData();
        }

        private void LoadData()
        {
            var animals = _animals.Find(a => a.CageId == null).ToList();
            AnimalBox.ItemsSource = animals;
            AnimalBox.DisplayMemberPath = "DisplayName";
            AnimalBox.SelectedValuePath = "Id";

            var cages = _cagesService.GetAllCages();
            CageBox.ItemsSource = cages;
            CageBox.DisplayMemberPath = "Location";
            CageBox.SelectedValuePath = "Id";
        }

        private void Assign_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalBox.SelectedValue == null || CageBox.SelectedValue == null)
            {
                MessageBox.Show("Select animal and cage!");
                return;
            }

            var success = _cagesService.AddAnimalToCage(
                AnimalBox.SelectedValue.ToString(),
                CageBox.SelectedValue.ToString()
            );

            if (!success)
            {
                MessageBox.Show("❌ Animal cannot be placed in this cage.");
                return;
            }

            MessageBox.Show("✅ Animal assigned!");
            DialogResult = true;
            Close();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}