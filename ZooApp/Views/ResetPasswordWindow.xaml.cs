using System.Windows;
using ZooApp.Services;
using ZooApp.Data;

namespace ZooApp.Views
{
    public partial class ResetPasswordWindow : Window
    {
        private readonly LoginService _service;

        public ResetPasswordWindow()
        {
            InitializeComponent();
            _service = new LoginService(new MongoDbContext("mongodb://localhost:27017", "test"));
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string newPassword = NewPasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Fill all fields!");
                return;
            }

            bool ok = _service.ResetPassword(login, newPassword);

            if (!ok)
            {
                MessageBox.Show("User not found!");
                return;
            }

            MessageBox.Show("Password updated!");
            Close();
        }
    }
}