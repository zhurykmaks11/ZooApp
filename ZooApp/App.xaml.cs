using System.Windows;
using ZooApp.Data;
using ZooApp.Views;
using MongoDB.Driver; 
using System; 
using System.Linq; 
using ZooApp.Services;

namespace ZooApp;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var mongoDbContext = new MongoDbContext("mongodb://localhost:27017", "test1"); 
            
            if (mongoDbContext.KeyUsers.Find(_ => true).FirstOrDefault() == null) 
            {
                MessageBox.Show("База даних порожня або не ініціалізована. Виконується початкове заповнення даними...", "Ініціалізація БД", MessageBoxButton.OK, MessageBoxImage.Information);
                ZooDbSeeder.Seed(mongoDbContext); 
                MessageBox.Show("База даних успішно ініціалізована.", "Ініціалізація БД", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("База даних вже містить дані. Пропускається початкове заповнення.", "Ініціалізація БД", MessageBoxButton.OK, MessageBoxImage.Information);
            }

          
        }
        catch (MongoConnectionException ex)
        {
            MessageBox.Show($"Помилка підключення до бази даних MongoDB.\nБудь ласка, переконайтесь, що сервер MongoDB запущено на 'mongodb://localhost:27017'.\nДеталі: {ex.Message}", "Помилка бази даних", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown(); 
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Виникла непередбачена помилка під час запуску програми: {ex.Message}", "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }
}