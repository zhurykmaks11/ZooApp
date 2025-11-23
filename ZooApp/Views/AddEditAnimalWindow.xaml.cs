using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZooApp.Models;
using ZooApp.Data;
using ZooApp.Services;

namespace ZooApp.Views
{
    public partial class AddEditAnimalWindow : Window
    {
        public Animal Animal { get; private set; }

        private readonly string _role;
        private readonly MongoDbContext _context;
        private readonly CagesService _cagesService;

        public AddEditAnimalWindow(string role, Animal existingAnimal = null)
        {
            InitializeComponent();

            _role = role;
            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _cagesService = new CagesService(_context);

            // створюємо або беремо існуючу модель
            Animal = existingAnimal ?? new Animal();

            LoadCages();

            // якщо редагування — заповнюємо поля
            if (existingAnimal != null)
            {
                NameBox.Text = existingAnimal.Name;
                SpeciesBox.Text = existingAnimal.Species;

                if (existingAnimal.Gender == "male")
                    GenderBox.SelectedIndex = 0;
                else if (existingAnimal.Gender == "female")
                    GenderBox.SelectedIndex = 1;

                BirthDatePicker.SelectedDate = existingAnimal.BirthDate;
                WeightBox.Text = existingAnimal.Weight.ToString(CultureInfo.InvariantCulture);
                HeightBox.Text = existingAnimal.Height.ToString(CultureInfo.InvariantCulture);

                if (existingAnimal.Type == "herbivore")
                    TypeBox.SelectedIndex = 0;
                else if (existingAnimal.Type == "predator")
                    TypeBox.SelectedIndex = 1;

                ClimateBox.Text = existingAnimal.ClimateZone;

                if (!string.IsNullOrEmpty(existingAnimal.CageId))
                    CageBox.SelectedValue = existingAnimal.CageId;
            }
        }

        // завантаження кліток тільки для вибору (без змін БД)
        private void LoadCages()
        {
            var cages = _cagesService.GetAllCages();
            CageBox.ItemsSource = cages;
            CageBox.DisplayMemberPath = "Location";
            CageBox.SelectedValuePath = "IdString";
        }

        // тільки перевірка введення double
        private void NumberOnly_TextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string fullText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            e.Handled = !double.TryParse(
                fullText.Replace(',', '.'),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out _);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            ResetBorders();

            void MarkInvalid(Control c)
            {
                c.BorderBrush = Brushes.Red;
                c.BorderThickness = new Thickness(2);
                valid = false;
            }

            // валідація
            if (string.IsNullOrWhiteSpace(NameBox.Text)) MarkInvalid(NameBox);
            if (string.IsNullOrWhiteSpace(SpeciesBox.Text)) MarkInvalid(SpeciesBox);
            if (GenderBox.SelectedItem == null) MarkInvalid(GenderBox);

            if (BirthDatePicker.SelectedDate == null || BirthDatePicker.SelectedDate > DateTime.Now)
                MarkInvalid(BirthDatePicker);

            if (!double.TryParse(WeightBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double weight) || weight <= 0)
                MarkInvalid(WeightBox);

            if (!double.TryParse(HeightBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double height) || height <= 0)
                MarkInvalid(HeightBox);

            if (TypeBox.SelectedItem == null) MarkInvalid(TypeBox);
            if (string.IsNullOrWhiteSpace(ClimateBox.Text)) MarkInvalid(ClimateBox);

            if (!valid)
            {
                MessageBox.Show("Будь ласка, виправ помилкові поля.", "Помилка", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // заповнюємо модель Animal (але НЕ пишемо в базу!)
            Animal.Name = NameBox.Text.Trim();
            Animal.Species = SpeciesBox.Text.Trim();
            Animal.Gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Animal.BirthDate = BirthDatePicker.SelectedDate!.Value;
            Animal.Weight = weight;
            Animal.Height = height;
            Animal.Type = (TypeBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Animal.ClimateZone = ClimateBox.Text.Trim();

            // просто зберігаємо обрану клітку в моделі (пізніше її обробимо в AnimalsWindow)
            Animal.CageId = CageBox.SelectedValue?.ToString();

            DialogResult = true;
            Close();
        }

        private void ResetBorders()
        {
            var brush = Brushes.Gray;
            foreach (var c in new Control[]
                     { NameBox, SpeciesBox, GenderBox, BirthDatePicker, WeightBox, HeightBox, TypeBox, ClimateBox })
            {
                c.BorderBrush = brush;
                c.BorderThickness = new Thickness(1);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
