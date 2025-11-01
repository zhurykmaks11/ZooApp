using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;
namespace ZooApp.Views
{
    public partial class FeedingWindow : Window
    {
        private readonly IMongoCollection<FeedingSchedule> _feeding;
        private readonly string _role;
        private readonly FeedingService _feedingService;


        public FeedingWindow(string role)
        {
            InitializeComponent();
            _role = role;
            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _feedingService = new FeedingService(context);
            LoadFeeding();
            if (_role == "Admin")
            {
                AddButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
                MessageBox.Show("You are logged in as operator. Editing and deleting are disabled.",
                    "Access Restricted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        } 

        private void LoadFeeding()
        {
            FeedingGrid.ItemsSource = _feedingService.GetAllFeedings();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddFeedingWindow();
            if (dialog.ShowDialog() == true)
            {
                _feedingService.AddFeeding(dialog.Feeding);
                LoadFeeding();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (FeedingGrid.SelectedItem is FeedingSchedule selected)
            {
                var editWindow = new EditFeedingWindow(selected);
                if (editWindow.ShowDialog() == true)
                {
                    _feedingService.UpdateFeeding(editWindow.Feeding);
                    LoadFeeding();
                }
            }
            else
            {
                MessageBox.Show("Select a feeding record to edit.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FeedingGrid.SelectedItem is FeedingSchedule selected)
            {
                _feedingService.DeleteFeeding(selected.Id);
                LoadFeeding();
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();
            FeedingGrid.ItemsSource = string.IsNullOrEmpty(query)
                ? _feedingService.GetAllFeedings()
                : _feedingService.SearchFeedings(query);
        }
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search...";
                SearchBox.Foreground = Brushes.Gray;
            }
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            var feedStats = _feedingService.GetFeedTypeStatistics();
            var seasons = _feedingService.GetAllSeasons();

            if (feedStats.Count == 0 && seasons.Count == 0)
            {
                MessageBox.Show("No feeding data available.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var message = new System.Text.StringBuilder();
            message.AppendLine("📊 Feeding Statistics\n");

            if (feedStats.Count > 0)
            {
                message.AppendLine("🔸 Total Feed by Type:");
                foreach (var kvp in feedStats)
                    message.AppendLine($"{kvp.Key}: {kvp.Value} kg");
                message.AppendLine();
            }
           
            

            if (seasons.Count > 0)
            {
                message.AppendLine("🌤 Total Feed by Season:");
                foreach (var season in seasons)
                {
                    double total = _feedingService.GetTotalFeedBySeason(season);
                    message.AppendLine($"{season}: {total} kg");
                }
            }

            MessageBox.Show(message.ToString(), "Feeding Statistics", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role).Show();
            this.Close();
        }

    }
}
