using System.Linq;
using System.Windows;
using ZooApp.Data;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class CageWindow : Window
    {
        private readonly CagesService _cagesService;
        private readonly string _role;
        private readonly string _username;
        
        public CageWindow(string role)
        {
            InitializeComponent();
            _role = role;
            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _cagesService = new CagesService(context);

            LoadCages();
        }
        private void EditCage_Click(object sender, RoutedEventArgs e)
        {
            if (CagesGrid.SelectedItem == null)
            {
                MessageBox.Show("Select a cage first.");
                return;
            }

            dynamic row = CagesGrid.SelectedItem;
            string cageId = row.Id.ToString();

            var win = new EditCageWindow(cageId);
            if (win.ShowDialog() == true)
                LoadCages();
        }

        private void DeleteCage_Click(object sender, RoutedEventArgs e)
        {
            if (CagesGrid.SelectedItem == null)
            {
                MessageBox.Show("Select a cage first.");
                return;
            }

            dynamic row = CagesGrid.SelectedItem;
            string cageId = row.Id.ToString();

            // Перевірка на тварин
            var cage = _cagesService.GetCage(cageId);
            if (cage.Animals.Count > 0)
            {
                MessageBox.Show("❌ Cannot delete cage — animals still inside!");
                return;
            }

            if (MessageBox.Show("Delete cage?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _cagesService.DeleteCage(cageId);
                MessageBox.Show("✅ Cage deleted.");
                LoadCages();
            }
        }

        private void LoadCages()
        {
            var cages = _cagesService.GetAllCages()
                .Select(c => new
                {
                    c.Id,
                    c.Location,
                    c.Size,
                    Heated = c.Heated ? "Yes" : "No",
                    c.Capacity,
                    AnimalCount = c.Animals.Count
                })
                .ToList();

            CagesGrid.ItemsSource = cages;
        }

        private void AddCage_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddCageWindow(_username);
            if (win.ShowDialog() == true)
                LoadCages();
        }

        private void MoveAnimal_Click(object sender, RoutedEventArgs e)
        {
            var win = new MoveAnimalWindow();
            if (win.ShowDialog() == true)
                LoadCages();
        }

        private void ViewAnimals_Click(object sender, RoutedEventArgs e)
        {
            if (CagesGrid.SelectedItem == null)
            {
                MessageBox.Show("Select a cage first.");
                return;
            }

            dynamic row = CagesGrid.SelectedItem;
            string cageId = row.Id.ToString();

            var win = new ViewCageAnimalsWindow(cageId);
            win.ShowDialog();
        }

        private void AssignAnimal_Click(object sender, RoutedEventArgs e)
        {
            var win = new AssignAnimalToCageWindow();
            if (win.ShowDialog() == true)
                LoadCages();
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            this.Close();
        }
    }
}