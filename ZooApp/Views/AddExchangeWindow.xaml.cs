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

        public AddExchangeWindow()
        {
            InitializeComponent();
            _context = new MongoDbContext("mongodb://localhost:27017", "test");

            Record = new ExchangeRecord();

            LoadAnimals();
            DateBox.SelectedDate = DateTime.Now;
        }

        public AddExchangeWindow(ExchangeRecord existing)
        {
            InitializeComponent();
            _context = new MongoDbContext("mongodb://localhost:27017", "test");

            Record = existing;

            LoadAnimals();

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

        private void LoadAnimals()
        {
            var animals = _context.Animals.Find(_ => true).ToList();
            AnimalBox.ItemsSource = animals;
            AnimalBox.DisplayMemberPath = "DisplayName";
            AnimalBox.SelectedValuePath = "Id";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
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

            Record.AnimalId = animal.Id;
            Record.AnimalName = $"{animal.Name} ({animal.Gender}, {age}y)";
            Record.ExchangeDate = DateBox.SelectedDate ?? DateTime.Now;
            Record.ExchangeType = type;
            Record.OtherZoo = ZooBox.Text.Trim();
            Record.Reason = ReasonBox.Text.Trim();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
