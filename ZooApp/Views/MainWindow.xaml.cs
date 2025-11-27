using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Services; 
namespace ZooApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly string _role;
        private readonly string _username;

        
        public MainWindow()
        {
            InitializeComponent();
            _role = "guest";
            _username = "guest";
            Title = "ZooApp — guest";
            RoleLabel.Text = $"Logged in as: {_role}";
            ApplyAccessRules();
        }
        
        public MainWindow(string role, string username)
        {
            InitializeComponent();

            _role = role.ToLower();           
            _username = username;

            Title = $"ZooApp — {_role}";
            RoleLabel.Text = $"Logged in as: {_role}";
            ApplyAccessRules();
        }

        
        private void ApplyAccessRules()
        {
           
            EmployeesButton.Visibility   = Visibility.Collapsed;
            AddUserButton.Visibility     = Visibility.Collapsed;
            LogsButton.Visibility        = Visibility.Collapsed;
            SqlConsoleButton.Visibility  = Visibility.Collapsed;
            SuppliersButton.Visibility   = Visibility.Visible;
            ExchangeButton.Visibility    = Visibility.Visible;

            switch (_role)
            {
                case "guest":
                    break;

                case "authorized":
                    break;

                case "operator":
                    SqlConsoleButton.Visibility = Visibility.Visible;
                    break;

                case "admin":
                    EmployeesButton.Visibility  = Visibility.Visible;
                    AddUserButton.Visibility    = Visibility.Visible;
                    LogsButton.Visibility       = Visibility.Visible;
                    SqlConsoleButton.Visibility = Visibility.Visible;
                    break;

                default:
                    // На всякий випадок — поводимось як guest
                    break;
            }
        }
        

        private void Animals_Click(object sender, RoutedEventArgs e)
        {
            var win = new AnimalsWindow(_role, _username);
            win.Show();
            Close();
        }

        private void Cages_Click(object sender, RoutedEventArgs e)
        {
            var win = new CageWindow(_role, _username);
            win.Show();
            Close();
        }

        private void Feeding_Click(object sender, RoutedEventArgs e)
        {
            var win = new FeedingWindow(_role, _username);
            win.Show();
            Close();
        }

        private void Medical_Click(object sender, RoutedEventArgs e)
        {
            var win = new MedicalWindow(_role, _username);
            win.Show();
            Close();
        }

        private void Suppliers_Click(object sender, RoutedEventArgs e)
        {
            var win = new SupplierWindow(_role, _username);
            win.Show();
            Close();
        }

        private void Exchange_Click(object sender, RoutedEventArgs e)
        {
            var win = new ExchangeWindow(_role, _username);
            win.Show();
            Close();
        }
        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            var win = new ReportsWindow(_role, _username);
            win.Show();
            this.Close();
        }

        private void ResetDb_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Очистити та пересоздати базу?", 
                    "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            var ctx = new MongoDbContext("mongodb://localhost:27017", "test");

            ctx.Animals.DeleteMany(_ => true);
            ctx.Cages.DeleteMany(_ => true);
            ctx.Employees.DeleteMany(_ => true);
            ctx.Feeds.DeleteMany(_ => true);
            ctx.Suppliers.DeleteMany(_ => true);
            ctx.FeedingSchedules.DeleteMany(_ => true);
            ctx.MedicalRecords.DeleteMany(_ => true);
            ctx.ExchangeRecords.DeleteMany(_ => true);
            ctx.Logs.DeleteMany(_ => true);

            ZooDbSeeder.Seed(ctx);

            MessageBox.Show("Базу пересотворено!");
        }

        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            var win = new EmployeeWindow(_role, _username);
            win.Show();
            Close();
        }

        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            var win = new LogsWindow(_role, _username);
            win.Show();
            Close();
        }

        private void SqlConsole_Click(object sender, RoutedEventArgs e)
        {
            var win = new SqlConsoleWindow(_role, _username);
            win.Show();
            Close();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddUserWindow();
            win.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            Close();
        }
    }
}
