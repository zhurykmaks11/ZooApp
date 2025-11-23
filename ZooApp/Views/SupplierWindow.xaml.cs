using System.Windows;
using ZooApp.Services;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class SupplierWindow : Window
    {
        private readonly SupplierService _service;
        private readonly LogService _log;

        private readonly string _role;
        private readonly string _username;

        public SupplierWindow(string role, string username)
        {
            InitializeComponent();

            _role = role;
            _username = username;

            var context = new MongoDbContext("mongodb://localhost:27017", "test");

            _service = new SupplierService(context);
            _log = new LogService(context);

            LoadData();
            ApplyRoleRules();
        }

        private void ApplyRoleRules()
        {
            if (_role == "operator")
            {
                AddButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                DeleteButton.IsEnabled = false;
            }
        }

        private void LoadData()
        {
            SuppliersGrid.ItemsSource = _service.GetAll();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddSupplierWindow();

            if (win.ShowDialog() == true)
            {
                _service.Add(win.Supplier);
                _log.Write(_username, "Add Supplier", $"Supplier={win.Supplier.Name}");

                LoadData();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersGrid.SelectedItem is not Supplier s)
            {
                MessageBox.Show("Select supplier!");
                return;
            }

            var win = new AddSupplierWindow(s);

            if (win.ShowDialog() == true)
            {
                _service.Update(win.Supplier);
                _log.Write(_username, "Edit Supplier", $"Supplier={win.Supplier.Name}");

                LoadData();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersGrid.SelectedItem is not Supplier s)
            {
                MessageBox.Show("Select supplier!");
                return;
            }

            _service.Delete(s.Id);
            _log.Write(_username, "Delete Supplier", $"Supplier={s.Name}");

            LoadData();
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string text = SearchBox.Text.Trim();
            SuppliersGrid.ItemsSource = _service.Search(text);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(_role, _username).Show();
            Close();
        }
    }
}
