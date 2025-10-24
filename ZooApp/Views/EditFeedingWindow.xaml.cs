using System.Windows;
using System.Windows.Input;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class EditFeedingWindow : Window
    {
        public FeedingSchedule Feeding { get; private set; }

        public EditFeedingWindow(FeedingSchedule feeding)
        {
            InitializeComponent();
            Feeding = feeding;

            AnimalNameBox.Text = feeding.AnimalName;
            FeedTypeBox.Text = feeding.FeedType;
            QuantityBox.Text = feeding.QuantityKg.ToString();
            FeedingTimeBox.Text = feeding.FeedingTime;
            SeasonBox.Text = feeding.Season;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(QuantityBox.Text, out double qty) || qty <= 0)
            {
                QuantityBox.BorderBrush = System.Windows.Media.Brushes.Red;
                MessageBox.Show("Quantity must be positive.");
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
    }
}