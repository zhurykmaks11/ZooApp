using System.Windows;
using ZooApp.Data;
using ZooApp.Models;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class ExchangeWindow : Window
    {
        private readonly ExchangeService _service;
        private readonly LogService _log;
        private readonly string _role;
        private readonly string _username;
        
        public ExchangeWindow(string role, string username)
        {
            InitializeComponent();

            _role = role.ToLower();
            _username = username;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");

            _service = new ExchangeService(context);
            _log = new LogService(context);

            LoadData();
            ApplyRoleRules();
        }

        private void ApplyRoleRules()
        {
            switch (_role.ToLower())
            {
                case "admin":
                    break; 

                case "operator":
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;

                case "authorized":
                case "guest":
                    AddButton.IsEnabled = false;
                    EditButton.IsEnabled = false;
                    DeleteButton.IsEnabled = false;
                    break;
            }
        }


        private void LoadData()
        {
            ExchangeGrid.ItemsSource = _service.GetAll();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddExchangeWindow();

            if (win.ShowDialog() == true)
            {
                _service.Add(win.Record);

                _log.Write(_username, "Add Exchange",
                    $"Animal={win.Record.AnimalName}, Type={win.Record.ExchangeType}");

                LoadData();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ExchangeGrid.SelectedItem is not ExchangeRecord selected)
            {
                MessageBox.Show("Select record!");
                return;
            }

            var win = new AddExchangeWindow(selected);

            if (win.ShowDialog() == true)
            {
                _service.Update(win.Record);

                _log.Write(_username, "Edit Exchange",
                    $"Animal={win.Record.AnimalName}, Type={win.Record.ExchangeType}");

                LoadData();
            }
        }
        private void Partners_Click(object sender, RoutedEventArgs e)
        {
            new ZooPartnersWindow().ShowDialog();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ExchangeGrid.SelectedItem is not ExchangeRecord selected)
            {
                MessageBox.Show("Select record!");
                return;
            }

            _service.Delete(selected.Id);

            _log.Write(_username, "Delete Exchange", $"Animal={selected.AnimalName}");

            LoadData();
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string text = SearchBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(text) || text == "Search...")
            {
                LoadData();
                return;
            }

            ExchangeGrid.ItemsSource = _service.Search(text);
        }

        private void Search_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void Search_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search...";
                SearchBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }
    }
}
