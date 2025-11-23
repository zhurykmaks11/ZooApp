using System.Windows;
using ZooApp.Data;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginService _loginService;
        private readonly LogService _log;
        
        public LoginWindow()
        {
            InitializeComponent();
            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _loginService = new LoginService(context);
            _log = new LogService(context);
        }
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var win = new ResetPasswordWindow();
            win.ShowDialog();
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
                _log.Write(user.Login, "Login", "User logged in successfully");

                var mainWindow = new MainWindow(user.Role, user.Login); 
                mainWindow.Show();
                this.Close();
            }
            else
            {
                _log.Write(username, "Failed Login", "Incorrect password or user not found");
                MessageBox.Show("Невірний логін або пароль.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}