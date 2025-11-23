using System.Windows;
using ZooApp.Services;
using ZooApp.Data;

namespace ZooApp.Views
{
    public partial class LogsWindow : Window
    {
        private readonly LogService _logService;
        private readonly string _username;
        private readonly string _role;
        
        public LogsWindow(string role, string username)
        {
            InitializeComponent();
            _role = role;
            _username = username;
            
            if (role != "admin")
            {
                MessageBox.Show("Access denied!");
                Close();
            }

            
            _logService = new LogService(new MongoDbContext("mongodb://localhost:27017", "test"));
            LoadData();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            this.Close();
        }
        private void LoadData()
        {
            LogsGrid.ItemsSource = _logService.GetAll();
        }
    }
}