using MongoDB.Driver;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class AddExchangeWindow : Window
    {
        public ExchangeRecord Record { get; private set; }

        private readonly MongoDbContext _context;

        // -------------------- ADD --------------------
        public AddExchangeWindow()
        {
            InitializeComponent();
            _context = new MongoDbContext("mongodb://localhost:27017", "test");

            LoadAnimals();
            DateBox.SelectedDate = DateTime.Now;
        }

        private void LoadAnimals()
        {
            var animals = _context.Animals.Find(_ => true).ToList();
            AnimalBox.ItemsSource = animals;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // ✅ Валідація
            if (AnimalBox.SelectedItem == null)
            {
                AnimalBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Select an animal!");
                return;
            }

            if (TypeBox.SelectedItem == null)
            {
                TypeBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Select exchange type!");
                return;
            }

            if (string.IsNullOrWhiteSpace(ZooBox.Text))
            {
                ZooBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter other zoo name!");
                return;
            }

            var animal = (Animal)AnimalBox.SelectedItem;
            var type = ((ComboBoxItem)TypeBox.SelectedItem).Content.ToString();

            int age = DateTime.Now.Year - animal.BirthDate.Year;

            Record = new ExchangeRecord
            {
                AnimalId = animal.Id,
                AnimalName = $"{animal.Name} ({animal.Gender}, {age}y)",
                ExchangeDate = DateBox.SelectedDate ?? DateTime.Now,
                ExchangeType = type,
                OtherZoo = ZooBox.Text.Trim(),
                Reason = ReasonBox.Text.Trim()
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // -------------------- EDIT --------------------
        public AddExchangeWindow(ExchangeRecord existing)
        {
            InitializeComponent();
            _context = new MongoDbContext("mongodb://localhost:27017", "test");

            LoadAnimals();

            Record = existing;

            // Заповнюємо поля
            AnimalBox.SelectedValue = existing.AnimalId;
            DateBox.SelectedDate = existing.ExchangeDate;
            ZooBox.Text = existing.OtherZoo;
            ReasonBox.Text = existing.Reason;

            foreach (ComboBoxItem item in TypeBox.Items)
            {
                if (item.Content.ToString() == existing.ExchangeType)
                {
                    TypeBox.SelectedItem = item;
                    break;
                }
            }
        }
    }
}
