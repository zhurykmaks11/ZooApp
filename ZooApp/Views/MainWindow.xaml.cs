using System.Windows;

namespace ZooApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly string _role;
        private readonly string _username;

        // Конструктор без параметрів (на всякий випадок)
        public MainWindow()
        {
            InitializeComponent();
            _role = "guest";
            _username = "guest";
            Title = "ZooApp — guest";
            RoleLabel.Text = $"Logged in as: {_role}";
            ApplyAccessRules();
        }

        // Основний конструктор
        public MainWindow(string role, string username)
        {
            InitializeComponent();

            _role = role.ToLower();           // приводимо до нижнього регістру
            _username = username;

            Title = $"ZooApp — {_role}";
            RoleLabel.Text = $"Logged in as: {_role}";
            ApplyAccessRules();
        }

        /// <summary>
        /// Правила доступу для ролей:
        /// admin      – повний доступ, SQL Console, Logs, Employees, AddUser
        /// operator   – CRUD-операції з даними, SQL Console, без керування користувачами/логами
        /// authorized – тільки перегляд + пошук, без SQL Console, без адміністрування
        /// guest      – тільки перегляд даних, мінімальне меню
        /// </summary>
        private void ApplyAccessRules()
        {
            // Спочатку ховаємо все "адмінське"
            EmployeesButton.Visibility   = Visibility.Collapsed;
            AddUserButton.Visibility     = Visibility.Collapsed;
            LogsButton.Visibility        = Visibility.Collapsed;
            SqlConsoleButton.Visibility  = Visibility.Collapsed;
            SuppliersButton.Visibility   = Visibility.Visible;
            ExchangeButton.Visibility    = Visibility.Visible;

            switch (_role)
            {
                case "guest":
                    // Гість може тільки дивитися дані:
                    // показуємо лише базові модулі
                    // (Animals, Cages, Feeding, Medical, Suppliers, Exchange)
                    // але в самих вікнах мають бути відключені кнопки Add/Edit/Delete
                    break;

                case "authorized":
                    // Авторизований – перегляд + пошук.
                    // Нема доступу до адміністрування і кастомної SQL-консолі
                    break;

                case "operator":
                    // Оператор – працює з даними (animals, cages, feeds, suppliers, exchange, medical)
                    // але не керує користувачами і не дивиться системні логи.
                    SqlConsoleButton.Visibility = Visibility.Visible; // оператор має доступ до SQL-консолі
                    break;

                case "admin":
                    // Адмін – бачить все
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

        // -------------------- Навігація --------------------

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
