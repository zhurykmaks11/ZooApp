using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class AddFeedingWindow : Window
    {
        private readonly LogService _log;
        private readonly MongoDbContext _context;
        private readonly string _username;

        public FeedingSchedule Feeding { get; private set; }

        public AddFeedingWindow(string username)
        {
            InitializeComponent();

            _username = username;

            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _log = new LogService(_context);

            LoadAnimals();

            FeedingTimeBox.Text = DateTime.Now.ToString("HH:mm");
        }

        private void LoadAnimals()
        {
            try
            {
                var animals = _context.Animals.Find(_ => true).ToList();
                AnimalComboBox.ItemsSource = animals;
                AnimalComboBox.DisplayMemberPath = "DisplayName";
                AnimalComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading animals: {ex.Message}");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an animal.");
                AnimalComboBox.BorderBrush = Brushes.Red;
                return;
            }

            if (string.IsNullOrWhiteSpace(FeedTypeBox.Text))
            {
                FeedTypeBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Feed type is required.");
                return;
            }

            if (!double.TryParse(QuantityBox.Text, out double qty) || qty <= 0 || qty > 1000)
            {
                QuantityBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Quantity must be between 0.1 and 1000 kg.");
                return;
            }

            if (!TimeSpan.TryParse(FeedingTimeBox.Text, out _))
            {
                FeedingTimeBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Feeding time must be in HH:mm format.");
                return;
            }

            if (SeasonComboBox.SelectedItem == null)
            {
                SeasonComboBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Please select a season.");
                return;
            }

            var animal = (Animal)AnimalComboBox.SelectedItem;
            var season = ((ComboBoxItem)SeasonComboBox.SelectedItem).Content.ToString();

            Feeding = new FeedingSchedule
            {
                AnimalId = animal.Id,
                AnimalName = animal.Name,
                FeedType = FeedTypeBox.Text.Trim(),
                QuantityKg = qty,
                FeedingTime = FeedingTimeBox.Text.Trim(),
                Season = season
            };

            

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !double.TryParse(e.Text, out _);
        }

        private void AnimalComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnimalComboBox.SelectedItem is Animal animal)
            {
                if (animal.Type == "herbivore")
                    FeedTypeBox.Text = "fruits";
                else if (animal.Type == "predator")
                    FeedTypeBox.Text = "meat";
            }
        }

        private void QuantityBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }
    }
}
