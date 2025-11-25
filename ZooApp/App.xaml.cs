using System.Windows;
using ZooApp.Data;
using ZooApp.Views;

namespace ZooApp;


public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var ctx = new MongoDbContext("mongodb://localhost:27017", "test");
        ZooDbSeeder.Seed(ctx);

        new LoginWindow().Show();
    }

}