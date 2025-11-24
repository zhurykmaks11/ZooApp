using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class FeedingWindow : Window
    {
        private readonly FeedingService _feedingService;
        private readonly LogService _log;
        private readonly string _role;
        private readonly string _username;

        public FeedingWindow(string role, string username)
        {
            InitializeComponent();

            _role = role.ToLower();
            _username = username;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _feedingService = new FeedingService(context);
            _log = new LogService(context);

            ApplyRole();
            LoadFeeding();
        }

        private void ApplyRole()
        {
            if (_role == "guest" || _role == "authorized")
            {
                AddButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;

                MessageBox.Show(
                    "You are logged in as a read-only user. Editing and deleting are disabled.",
                    "Access Restricted",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void LoadFeeding()
        {
            FeedingGrid.ItemsSource = _feedingService.GetAllFeedings();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddFeedingWindow(_username);
            if (dialog.ShowDialog() == true)
            {
                _feedingService.AddFeeding(dialog.Feeding);

                _log.Write(_username, "Add Feeding",
                    $"Animal={dialog.Feeding.AnimalName}, Feed={dialog.Feeding.FeedType}, Qty={dialog.Feeding.QuantityKg}");

                LoadFeeding();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (FeedingGrid.SelectedItem is not FeedingSchedule selected)
            {
                MessageBox.Show("Select feeding record to edit.");
                return;
            }

            var dialog = new EditFeedingWindow(selected, _username);

            if (dialog.ShowDialog() == true)
            {
                _feedingService.UpdateFeeding(dialog.Feeding);

                _log.Write(_username, "Edit Feeding",
                    $"Animal={dialog.Feeding.AnimalName}, Feed={dialog.Feeding.FeedType}, Qty={dialog.Feeding.QuantityKg}");

                LoadFeeding();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (FeedingGrid.SelectedItem is not FeedingSchedule selected)
            {
                MessageBox.Show("Select a record.");
                return;
            }

            if (MessageBox.Show("Delete record?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _feedingService.DeleteFeeding(selected.Id);

                _log.Write(_username, "Delete Feeding",
                    $"Animal={selected.AnimalName}, Feed={selected.FeedType}");

                LoadFeeding();
            }
        }

        private void Clean_Click(object sender, RoutedEventArgs e)
        {
            int removed = _feedingService.CleanupOrphanFeedings();

            _log.Write(_username, "Clean Feedings", $"Removed={removed}");

            MessageBox.Show($"Removed {removed} orphan feeding records.");
            LoadFeeding();
        }

        private void Summary_Click(object sender, RoutedEventArgs e)
        {
            var feeds = _feedingService.GetAnimalFeedInfo();
            if (feeds.Count == 0)
            {
                MessageBox.Show("No feeding data.");
                return;
            }

            _log.Write(_username, "Feeding Summary", "Viewed summary");

            var message = string.Join("\n",
                feeds.Select(f => $"{f.AnimalName} → {f.FeedType} ({f.Quantity} кг)"));

            MessageBox.Show(message, "Feeding Summary");
        }

        private void Stats_Click(object sender, RoutedEventArgs e)
        {
            var feedStats = _feedingService.GetFeedTypeStatistics();
            var seasons = _feedingService.GetAllSeasons();

            if (feedStats.Count == 0 && seasons.Count == 0)
            {
                MessageBox.Show("No data.");
                return;
            }

            _log.Write(_username, "Feed Stats", "Viewed statistics");

            var text = "📊 Feeding Statistics:\n\n";

            text += "🔸 By Feed Type:\n";
            foreach (var kvp in feedStats)
                text += $"{kvp.Key}: {kvp.Value} kg\n";

            text += "\n🌤 By Season:\n";
            foreach (var season in seasons)
            {
                double total = _feedingService.GetTotalFeedBySeason(season);
                text += $"{season}: {total} kg\n";
            }

            MessageBox.Show(text, "Feeding Stats");
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string q = SearchBox.Text.Trim().ToLower();

            FeedingGrid.ItemsSource =
                string.IsNullOrEmpty(q)
                ? _feedingService.GetAllFeedings()
                : _feedingService.SearchFeedings(q);
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
        private void FilterAnimals_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FeedFilterWindow();

            if (dialog.ShowDialog() == true)
            {
                var animals = _feedingService.GetUniqueAnimalsByFeedAndSeason(dialog.FeedType, dialog.Season);

                if (animals.Count == 0)
                {
                    MessageBox.Show("No animals match this filter.");
                    return;
                }

                var list = string.Join("\n", animals);

                MessageBox.Show(
                    $"Animals that eat '{dialog.FeedType}' in '{dialog.Season}':\n\n{list}",
                    "Filtered Animals",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                _log.Write(_username,
                    "Filter Animals",
                    $"Feed={dialog.FeedType}, Season={dialog.Season}, Count={animals.Count}");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }
    }
}
