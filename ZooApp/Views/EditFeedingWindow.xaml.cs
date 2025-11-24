using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class EditFeedingWindow : Window
    {
        private readonly LogService _log;
        private readonly string _username;

        public FeedingSchedule Feeding { get; private set; }

        public EditFeedingWindow(FeedingSchedule feeding, string username)
        {
            InitializeComponent();
            Feeding = feeding;

            _username = username;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _log = new LogService(context);

            AnimalNameBox.Text = feeding.AnimalName;
            FeedTypeBox.Text = feeding.FeedType;
            QuantityBox.Text = feeding.QuantityKg.ToString();
            FeedingTimeBox.Text = feeding.FeedingTime;
            SeasonBox.Text = feeding.Season;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AnimalNameBox.Text))
            {
                AnimalNameBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Animal name is required.");
                return;
            }

            if (!double.TryParse(QuantityBox.Text, out double qty) || qty <= 0 || qty > 1000)
            {
                QuantityBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Quantity must be between 0 and 1000 kg.");
                return;
            }

            if (!TimeSpan.TryParse(FeedingTimeBox.Text, out _))
            {
                FeedingTimeBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Invalid time format.");
                return;
            }

            Feeding.AnimalName = AnimalNameBox.Text.Trim();
            Feeding.FeedType = FeedTypeBox.Text.Trim();
            Feeding.QuantityKg = qty;
            Feeding.FeedingTime = FeedingTimeBox.Text.Trim();
            Feeding.Season = SeasonBox.Text.Trim();
            

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

        private void QuantityBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }
    }
}
