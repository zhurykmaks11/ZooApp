using System.Windows;

namespace ZooApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly string _role;

        // Конструктор без параметрів
        public MainWindow()
        {
            InitializeComponent();
            _role = "guest";
            RoleLabel.Text = $"Logged in as: {_role}";
            ApplyAccessRules();
        }

        // Конструктор із параметром role
        public MainWindow(string role)
        {
            InitializeComponent();
            _role = role.ToLower(); // робимо нечутливим до регістру
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
                    break;

                case "operator":
                    AddUserButton.Visibility = Visibility.Collapsed;
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
                    // Якщо раптом якась інша роль
                    EmployeesButton.Visibility = Visibility.Collapsed;
                    ExchangeButton.Visibility = Visibility.Collapsed;
                    AddUserButton.Visibility = Visibility.Collapsed;
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
            var win = new MedicalWindow();
            win.Show();
            this.Close();
        }

        private void Exchange_Click(object sender, RoutedEventArgs e)
        {
            var win = new ExchangeWindow();
            win.Show();
            this.Close();
        }

        private void Employees_Click(object sender, RoutedEventArgs e)
        {
            var win = new EmployeesWindow();
            win.Show();
            this.Close();
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
    }
}
