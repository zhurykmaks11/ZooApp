using System.Windows;
using ZooApp.Data;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginService _loginService;

        public LoginWindow()
        {
            InitializeComponent();

            // 🔗 Підключення до MongoDB
            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _loginService = new LoginService(context);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = LoginBox.Text.Trim();
            var password = PasswordBox.Password.Trim();

            var user = _loginService.Authenticate(username, password);

            if (user != null)
            {
                MessageBox.Show($"Вітаємо, {user.Login}! Ваша роль: {user.Role}",
                    "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

                var mainWindow = new MainWindow(user.Role); // ✅ передаємо реальну роль
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}