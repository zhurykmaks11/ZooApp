using System.Windows;

namespace ZooApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly string _role;
        private readonly string _username;
        
        // Конструктор без параметрів
        public MainWindow()
        {
            InitializeComponent();
            _role = "guest";
            RoleLabel.Text = $"Logged in as: {_role}";
            ApplyAccessRules();
        }

        // Конструктор із параметром role
        public MainWindow(string role, string username)
        {
            InitializeComponent();
            _role = role.ToLower(); 
            _username = username;
            Title = $"ZooApp — {_role}";
            RoleLabel.Text = $"Logged in as: {_role}";
            ApplyAccessRules();
        }

        // 🔒 Правила доступу для різних ролей
        
        private void ApplyAccessRules()
        {
            switch (_role.ToLower())
            {
                case "guest":
                    EmployeesButton.Visibility = Visibility.Collapsed;
                    ExchangeButton.Visibility = Visibility.Collapsed;
                    AddUserButton.Visibility = Visibility.Collapsed;
                    SuppliersButton.Visibility = Visibility.Collapsed;
                    break;

                case "operator":
                    AddUserButton.Visibility = Visibility.Collapsed;
                    SuppliersButton.Visibility = Visibility.Collapsed;
                    break;

                case "authorized":
                    AddUserButton.Visibility = Visibility.Collapsed;
                    break;

                case "admin":
                    EmployeesButton.Visibility = Visibility.Visible;
                    ExchangeButton.Visibility = Visibility.Visible;
                    AddUserButton.Visibility = Visibility.Visible;
                    break;

                default:
                    EmployeesButton.Visibility = Visibility.Collapsed;
                    ExchangeButton.Visibility = Visibility.Collapsed;
                    AddUserButton.Visibility = Visibility.Collapsed;
                    SuppliersButton.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Animals_Click(object sender, RoutedEventArgs e)
        {
            var win = new AnimalsWindow(_role);
            win.Show();
            this.Close();
        }

        private void Feeding_Click(object sender, RoutedEventArgs e)
        {
            var win = new FeedingWindow(_role);
            win.Show();
            this.Close();
        }

        private void Medical_Click(object sender, RoutedEventArgs e)
        {
            var win = new MedicalWindow(_role);
            win.Show();
            this.Close();
        }

        private void Exchange_Click(object sender, RoutedEventArgs e)
        {
            new ExchangeWindow(_role, _username).Show();
            this.Close();
        }
        

        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            var win = new EmployeeWindow(_role);
            win.Show();
            this.Close();
        }
        
        private void Suppliers_Click(object sender, RoutedEventArgs e)
        {
            var win = new SupplierWindow(_role, _username);
            win.Show();
            this.Close();
        }
        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            new LogsWindow(_role, _username).Show();
            Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddUserWindow();
            win.ShowDialog();
        }
        
        private void Cages_Click(object sender, RoutedEventArgs e)
        {
            var cageWindow = new CageWindow(_role);
            cageWindow.Show();
            this.Close();
        }

    }
}
