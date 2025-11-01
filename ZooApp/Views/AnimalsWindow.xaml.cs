using System.Linq;
using System.Windows;
using MongoDB.Driver;
using ZooApp.Data;
using ZooApp.Models;
using System.Windows.Media;
namespace ZooApp.Views
{
    public partial class AnimalsWindow : Window
    {
        private readonly IMongoCollection<Animal> _animals;
        private readonly string _role;

        public AnimalsWindow(string role)
        {
            InitializeComponent();

            _role = role;
            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _animals = context.Animals;
            LoadAnimals();
        }

        private void LoadAnimals()
        {
            var list = _animals.Find(_ => true).ToList();
            AnimalsGrid.ItemsSource = list;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditAnimalWindow(_role);
            if (window.ShowDialog() == true)
            {
                _animals.InsertOne(window.Animal);
                LoadAnimals();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is Animal selected)
            {
                var window = new AddEditAnimalWindow(_role, selected);
                if (window.ShowDialog() == true)
                {
                    _animals.ReplaceOne(a => a.Id == selected.Id, window.Animal);
                    LoadAnimals();
                }
            }
            else
            {
                MessageBox.Show("Select an animal to edit.");
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // 🧠 Шукаємо, чи вже є відкрите головне вікно
            foreach (Window w in Application.Current.Windows)
            {
                if (w is MainWindow main)
                {
                    main.Show();
                    this.Close();
                    return;
                }
            }

            // 🆕 Якщо ні — створюємо нове головне меню з поточною роллю
            new MainWindow(_role).Show();
            this.Close();
        }



        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsGrid.SelectedItem is Animal selected)
            {
                if (MessageBox.Show($"Видалити {selected.Name}?", "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _animals.DeleteOne(a => a.Id == selected.Id);
                    LoadAnimals();
                }
            }
            else
            {
                MessageBox.Show("Оберіть тварину для видалення.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                LoadAnimals();
                return;
            }

            var list = _animals.Find(a => a.Name.ToLower().Contains(query)).ToList();
            AnimalsGrid.ItemsSource = list;
        }
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Search by name...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Search by name...";
                SearchBox.Foreground = Brushes.Gray;
            }
        }
        

    }
}
