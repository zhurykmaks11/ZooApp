using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZooApp.Models;
using ZooApp.Data;
using MongoDB.Bson;
using ZooApp.Services;
using MongoDB.Driver;
namespace ZooApp.Views
{
    public partial class AddEditAnimalWindow : Window
    {
        public Animal Animal { get; private set; }
        private readonly string _role;
        private readonly CagesService _cageService;
        private readonly MongoDbContext _context;
        private readonly IMongoCollection<Animal> _animalsCollection;


        public AddEditAnimalWindow(string role, Animal existingAnimal = null)
        {
            InitializeComponent();
            _role = role;
            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _cageService = new CagesService(_context);
            _animalsCollection = _context.Animals;
            // спочатку створюємо/присвоюємо
            Animal = existingAnimal ?? new Animal();

            LoadCages(); // тепер можна безпечно читати Animal.CageId

            if (existingAnimal != null)
            {
                Animal = existingAnimal;
                NameBox.Text = existingAnimal.Name;
                SpeciesBox.Text = existingAnimal.Species;
                GenderBox.SelectedItem = existingAnimal.Gender == "male" ? GenderBox.Items[0] : GenderBox.Items[1];
                BirthDatePicker.SelectedDate = existingAnimal.BirthDate;
                WeightBox.Text = existingAnimal.Weight.ToString(CultureInfo.InvariantCulture);
                HeightBox.Text = existingAnimal.Height.ToString(CultureInfo.InvariantCulture);
                TypeBox.SelectedItem = existingAnimal.Type == "herbivore" ? TypeBox.Items[0] : TypeBox.Items[1];
                ClimateBox.Text = existingAnimal.ClimateZone;

                if (!string.IsNullOrEmpty(existingAnimal.CageId))
                    CageBox.SelectedValue = existingAnimal.CageId;
            }
            else
                Animal = new Animal();
        }

        private void NumberOnly_TextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
            
            
            
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

            if (string.IsNullOrWhiteSpace(NameBox.Text)) MarkInvalid(NameBox);
            if (string.IsNullOrWhiteSpace(SpeciesBox.Text)) MarkInvalid(SpeciesBox);
            if (GenderBox.SelectedItem == null) MarkInvalid(GenderBox);
            if (BirthDatePicker.SelectedDate == null || BirthDatePicker.SelectedDate > DateTime.Now) MarkInvalid(BirthDatePicker);
            if (!double.TryParse(WeightBox.Text, out double weight) || weight <= 0) MarkInvalid(WeightBox);
            if (!double.TryParse(HeightBox.Text, out double height) || height <= 0) MarkInvalid(HeightBox);
            if (TypeBox.SelectedItem == null) MarkInvalid(TypeBox);
            if (string.IsNullOrWhiteSpace(ClimateBox.Text)) MarkInvalid(ClimateBox);

            if (!valid)
            {
                MessageBox.Show("Fix highlighted fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ✅ Save animal data
            Animal.Name = NameBox.Text.Trim();
            Animal.Species = SpeciesBox.Text.Trim();
            Animal.Gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Animal.BirthDate = BirthDatePicker.SelectedDate.Value;
            Animal.Weight = weight;
            Animal.Height = height;
            Animal.Type = (TypeBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Animal.ClimateZone = ClimateBox.Text.Trim();

            // ✅ Clean ID for new animal (so MongoDB generates new one)
            if (string.IsNullOrEmpty(Animal.Id))
                Animal.Id = null;

            // ✅ Cage assignment (optional)
            string cageId = CageBox.SelectedValue?.ToString();
            if (!string.IsNullOrEmpty(cageId))
            {
                // If editing an animal that already has cage
                if (!string.IsNullOrEmpty(Animal.CageId) && Animal.CageId != cageId)
                {
                    _cageService.RemoveAnimalFromCage(Animal.Id, Animal.CageId);
                }

                if (!_cageService.AddAnimalToCage(Animal.Id, cageId))
                {
                    MessageBox.Show("Cannot assign cage (rules/space). Animal saved without cage.", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    Animal.CageId = cageId;
            }

            DialogResult = true;
            Close();
        }

        private void LoadCages()
        {
            var cages = _cageService.GetAllCages();
            CageBox.ItemsSource = cages;
        }


        private void ResetBorders()
        {
            Brush b = Brushes.Gray;
            foreach (var c in new Control[] { NameBox, SpeciesBox, GenderBox, BirthDatePicker, WeightBox, HeightBox, TypeBox, ClimateBox })
            {
                c.BorderBrush = b;
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
