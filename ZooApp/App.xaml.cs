using System.Windows;
using ZooApp.Data;
using ZooApp.Views;
using MongoDB.Driver; 
using System; 
using System.Linq; 
using ZooApp.Services; // Переконайтеся, що цей using присутній

namespace ZooApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // Ініціалізуємо контекст бази даних ОДИН РАЗ при старті додатку
            var mongoDbContext = new MongoDbContext("mongodb://localhost:27017", "test1"); 
            
            // ⭐ Логіка перевірки та ініціалізації бази даних
            // Перевіряємо, чи колекція KeyUsers порожня.
            // Якщо вона порожня, це означає, що база даних не ініціалізована або була очищена.
            if (mongoDbContext.KeyUsers.Find(_ => true).FirstOrDefault() == null) 
            {
                MessageBox.Show("База даних порожня або не ініціалізована. Виконується початкове заповнення даними...", "Ініціалізація БД", MessageBoxButton.OK, MessageBoxImage.Information);
                ZooDbSeeder.Seed(mongoDbContext); // Викликаємо початкове заповнення даних
                MessageBox.Show("База даних успішно ініціалізована.", "Ініціалізація БД", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Якщо дані вже існують (наприклад, є хоча б один KeyUser),
                // то пропускаємо початкове заповнення, щоб зберегти існуючі дані.
                MessageBox.Show("База даних вже містить дані. Пропускається початкове заповнення.", "Ініціалізація БД", MessageBoxButton.OK, MessageBoxImage.Information);
            }

          
        }
        catch (MongoConnectionException ex)
        {
            // Обробка помилки, якщо не вдалося підключитися до сервера MongoDB.
            // Це критична помилка, яка зупиняє додаток.
            MessageBox.Show($"Помилка підключення до бази даних MongoDB.\nБудь ласка, переконайтесь, що сервер MongoDB запущено на 'mongodb://localhost:27017'.\nДеталі: {ex.Message}", "Помилка бази даних", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown(); 
        }
        catch (Exception ex)
        {
            // Загальна обробка будь-яких інших непередбачених винятків при старті.
            MessageBox.Show($"Виникла непередбачена помилка під час запуску програми: {ex.Message}", "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }
}