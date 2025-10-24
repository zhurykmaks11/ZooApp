using System.Windows;
using System.Windows.Controls;
using ZooApp.Data;
using ZooApp.Services;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class AddUserWindow : Window
    {
        private readonly LoginService _loginService;

        public AddUserWindow()
        {
            InitializeComponent();

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _loginService = new LoginService(context);
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string role = (RoleBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                MessageBox.Show("Заповніть усі поля!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool added = _loginService.AddUser(login, password, role);

            if (added)
            {
                MessageBox.Show("✅ Користувача додано успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("⚠️ Користувач із таким логіном уже існує!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}