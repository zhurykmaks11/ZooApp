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
        private readonly IMongoCollection<KeyUser> _users;

        public LoginService(MongoDbContext context)
        {
            _context = context;
            _users = context.KeyUsers; // ⬅ ВАЖЛИВО
        }

        public KeyUser Authenticate(string username, string password)
        {
            var user = _users.Find(u => u.Login == username).FirstOrDefault();

            if (user == null)
                return null;

            var hashedPassword = HashPassword(password);
            return user.Password == hashedPassword ? user : null;
        }

        public bool ResetPassword(string login, string newPassword)
        {
            var user = _users.Find(u => u.Login == login).FirstOrDefault();

            if (user == null)
                return false;

            user.Password = HashPassword(newPassword);

            _users.ReplaceOne(u => u.Id == user.Id, user);
            return true;
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
            var exists = _users.Find(u => u.Login == login).FirstOrDefault();
            if (exists != null)
                return false;

            var hashed = HashPassword(password);

            var user = new KeyUser
            {
                Login = login,
                Password = hashed,
                Role = role
            };

            _users.InsertOne(user);
            return true;
        }
    }
}