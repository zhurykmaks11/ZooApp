using System.Linq;
using System.Windows;
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
                var dialog = new EditFeedingWindow(selected);
                if (dialog.ShowDialog() == true)
                {
                    _feedingService.UpdateFeeding(dialog.Feeding);
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

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role).Show();
            this.Close();
        }
    }
}
