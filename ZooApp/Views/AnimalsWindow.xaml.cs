using System.Linq;
using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class AnimalsWindow : Window
    {
        private readonly IMongoCollection<Animal> _animals;

        public AnimalsWindow()
        {
            InitializeComponent();

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _animals = context.Animals;
            LoadAnimals();
        }

        private void LoadAnimals()
        {
            var list = _animals.Find(_ => true).ToList();
            AnimalsGrid.ItemsSource = list;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var newAnimal = new Animal
            {
                Name = "New Animal",
                Species = "Unknown",
                Gender = "Unknown",
                BirthDate = System.DateTime.Now,
                Weight = 0,
                Height = 0,
                Type = "herbivore",
                ClimateZone = "temperate"
            };

            _animals.InsertOne(newAnimal);
            LoadAnimals();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is Animal selected)
            {
                selected.Name = "Updated Animal";
                _animals.ReplaceOne(a => a.Id == selected.Id, selected);
                LoadAnimals();
            }
            else
            {
                MessageBox.Show("Select an animal to edit.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is Animal selected)
            {
                _animals.DeleteOne(a => a.Id == selected.Id);
                LoadAnimals();
            }
            else
            {
                MessageBox.Show("Select an animal to delete.");
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                LoadAnimals();
                return;
            }

            var list = _animals.Find(a => a.Name.ToLower().Contains(query)).ToList();
            AnimalsGrid.ItemsSource = list;
        }
    }
}
