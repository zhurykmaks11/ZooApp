using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Services
{
    public class LoginService
    {
        private readonly MongoDbContext _context;

        public LoginService(MongoDbContext context)
        {
            _context = context;
        }

        public KeyUser Authenticate(string username, string password)
        {
            // Знаходимо користувача по логіну
            var user = _context.KeyUsers.Find(u => u.Login == username).FirstOrDefault();

            if (user == null)
                return null;

            // Хешуємо введений пароль і порівнюємо
            var hashedPassword = HashPassword(password);
            return user.Password == hashedPassword ? user : null;
        }

        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            }
        }
        public bool AddUser(string login, string password, string role)
        {
            // Перевіряємо, чи такий користувач вже існує
            var existing = _context.KeyUsers.Find(u => u.Login == login).FirstOrDefault();
            if (existing != null)
                return false;

            // Хешуємо пароль
            var hashed = HashPassword(password);

            var user = new KeyUser
            {
                Login = login,
                Password = hashed,
                Role = role
            };

            _context.KeyUsers.InsertOne(user);
            return true;
        }

    }
}