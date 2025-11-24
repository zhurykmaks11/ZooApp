using System.Linq;
using System.Windows;
using System.Windows.Media;
using ZooApp.Data;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class ZooPartnersWindow : Window
    {
        private readonly ExchangeService _service;

        public ZooPartnersWindow()
        {
            InitializeComponent();

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _service = new ExchangeService(context);

            LoadAllZoos();
        }

        private void LoadAllZoos()
        {
            var zoos = _service.GetPartnerZoos();
            ZooList.ItemsSource = zoos;
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string species = SpeciesBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(species) || species == "Filter by species...")
            {
                LoadAllZoos();
                return;
            }

            var zoos = _service.GetPartnerZoosBySpecies(species);

            if (zoos.Count == 0)
                MessageBox.Show("No exchanges for this species.");

            ZooList.ItemsSource = zoos;
        }

        private void SpeciesBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SpeciesBox.Text == "Filter by species...")
            {
                SpeciesBox.Text = "";
                SpeciesBox.Foreground = Brushes.Black;
            }
        }

        private void SpeciesBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SpeciesBox.Text))
            {
                SpeciesBox.Text = "Filter by species...";
                SpeciesBox.Foreground = Brushes.Gray;
            }
        }
    }
}