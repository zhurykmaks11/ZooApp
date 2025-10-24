using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class AddEditAnimalWindow : Window
    {
        public Animal Animal { get; private set; }
        private readonly string _role;

        public AddEditAnimalWindow(string role, Animal existingAnimal = null)
        {
            InitializeComponent();
            _role = role;
            
            if (existingAnimal != null)
            {
                Animal = existingAnimal;
                NameBox.Text = existingAnimal.Name;
                SpeciesBox.Text = existingAnimal.Species;
                GenderBox.Text = existingAnimal.Gender;
                BirthDatePicker.SelectedDate = existingAnimal.BirthDate;
                WeightBox.Text = existingAnimal.Weight.ToString(CultureInfo.InvariantCulture);
                HeightBox.Text = existingAnimal.Height.ToString(CultureInfo.InvariantCulture);
                TypeBox.Text = existingAnimal.Type;
                ClimateBox.Text = existingAnimal.ClimateZone;
            }
            else
            {
                Animal = new Animal();
            }
        }
        private void NumberOnly_TextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
            
            
            
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            // 🔧 Скидаємо підсвічування
            ResetBorders();

            void MarkInvalid(Control ctrl)
            {
                ctrl.BorderBrush = Brushes.Red;
                ctrl.BorderThickness = new Thickness(2);
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text))
                MarkInvalid(NameBox);

            if (string.IsNullOrWhiteSpace(SpeciesBox.Text))
                MarkInvalid(SpeciesBox);

            if (GenderBox.SelectedItem == null)
                MarkInvalid(GenderBox);

            if (BirthDatePicker.SelectedDate == null)
                MarkInvalid(BirthDatePicker);
            else if (BirthDatePicker.SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Дата народження не може бути в майбутньому!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                MarkInvalid(BirthDatePicker);
            }

            if (!double.TryParse(WeightBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double weight) || weight <= 0)
                MarkInvalid(WeightBox);

            if (!double.TryParse(HeightBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double height) || height <= 0)
                MarkInvalid(HeightBox);

            if (TypeBox.SelectedItem == null)
                MarkInvalid(TypeBox);

            if (string.IsNullOrWhiteSpace(ClimateBox.Text))
                MarkInvalid(ClimateBox);

            if (!isValid)
            {
                MessageBox.Show("Будь ласка, виправте помилки перед збереженням.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ✅ Якщо все валідно — зберігаємо
            Animal.Name = NameBox.Text.Trim();
            Animal.Species = SpeciesBox.Text.Trim();
            Animal.Gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Animal.BirthDate = BirthDatePicker.SelectedDate ?? DateTime.Now;
            Animal.Weight = weight;
            Animal.Height = height;
            Animal.Type = (TypeBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Animal.ClimateZone = ClimateBox.Text.Trim();

            DialogResult = true;
            Close();
        }
        
        private void ResetBorders()
        {
            Brush defaultBrush = Brushes.Gray;
            double defaultThickness = 1;

            foreach (var control in new Control[] { NameBox, SpeciesBox, GenderBox, BirthDatePicker, WeightBox, HeightBox, TypeBox, ClimateBox })
            {
                control.BorderBrush = defaultBrush;
                control.BorderThickness = new Thickness(defaultThickness);
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
