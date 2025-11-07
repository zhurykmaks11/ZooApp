using System.Linq;
using System.Windows;
using MongoDB.Bson;
using MongoDB.Driver;
using ZooApp.Data;

namespace ZooApp.Views
{
    public partial class ViewCageAnimalsWindow : Window
    {
        public ViewCageAnimalsWindow(string cageId)
        {
            InitializeComponent();

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            var animals = context.Animals.Find(a => a.CageId == cageId).ToList();

            AnimalsGrid.ItemsSource = animals;
        }
    }
}