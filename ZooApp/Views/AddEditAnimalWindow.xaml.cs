using System;
using System.Globalization;
using System.Linq;
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
        private readonly AnimalsService _animalsService;

        public AddEditAnimalWindow(string role, Animal existingAnimal = null)
        {
            InitializeComponent();

            _role = role;
            _context = new MongoDbContext("mongodb://localhost:27017", "test");
            _cagesService = new CagesService(_context);
            _animalsService = new AnimalsService(_context);

            Animal = existingAnimal ?? new Animal();

            LoadCages();
            LoadParents();

            if (existingAnimal != null)
                FillFields(existingAnimal);
            else
                IsolationReasonBox.IsEnabled = false;
        }

        private void LoadCages()
        {
            var cages = _cagesService.GetAllCages();
            CageBox.ItemsSource = cages;
            CageBox.DisplayMemberPath = "Location";
            CageBox.SelectedValuePath = "IdString";
        }

        private void LoadParents()
        {
            var animals = _animalsService.GetAllAnimals();

            MotherBox.ItemsSource = animals
                .Where(a => a.Gender == "female")
                .ToList();

            FatherBox.ItemsSource = animals
                .Where(a => a.Gender == "male")
                .ToList();
        }

        private void FillFields(Animal a)
        {
            NameBox.Text = a.Name;
            SpeciesBox.Text = a.Species;

            if (a.Gender == "male")
                GenderBox.SelectedIndex = 0;
            else if (a.Gender == "female")
                GenderBox.SelectedIndex = 1;

            BirthDatePicker.SelectedDate = a.BirthDate;
            WeightBox.Text = a.Weight.ToString(CultureInfo.InvariantCulture);
            HeightBox.Text = a.Height.ToString(CultureInfo.InvariantCulture);

            if (a.Type == "herbivore")
                TypeBox.SelectedIndex = 0;
            else if (a.Type == "predator")
                TypeBox.SelectedIndex = 1;

            ClimateBox.Text = a.ClimateZone;

            WarmShelterBox.IsChecked = a.NeedsWarmShelter;

            IsolatedBox.IsChecked = a.IsIsolated;
            IsolationReasonBox.Text = a.IsolationReason;
            IsolationReasonBox.IsEnabled = a.IsIsolated;

            MotherBox.SelectedValue = a.MotherId;
            FatherBox.SelectedValue = a.FatherId;

            if (!string.IsNullOrEmpty(a.CageId))
                CageBox.SelectedValue = a.CageId;
        }

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

        private bool ValidateForm()
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

            if (BirthDatePicker.SelectedDate == null ||
                BirthDatePicker.SelectedDate > DateTime.Now)
                MarkInvalid(BirthDatePicker);

            if (!double.TryParse(WeightBox.Text.Replace(',', '.'),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double weight) || weight <= 0)
                MarkInvalid(WeightBox);

            if (!double.TryParse(HeightBox.Text.Replace(',', '.'),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double height) || height <= 0)
                MarkInvalid(HeightBox);

            if (TypeBox.SelectedItem == null) MarkInvalid(TypeBox);
            if (string.IsNullOrWhiteSpace(ClimateBox.Text)) MarkInvalid(ClimateBox);

            if (IsolatedBox.IsChecked == true &&
                string.IsNullOrWhiteSpace(IsolationReasonBox.Text))
                MarkInvalid(IsolationReasonBox);

            if (!valid)
            {
                MessageBox.Show("Будь ласка, виправ виділені поля.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            return valid;
        }

        private void ResetBorders()
        {
            var controls = new Control[]
            {
                NameBox, SpeciesBox, GenderBox, BirthDatePicker,
                WeightBox, HeightBox, TypeBox, ClimateBox, IsolationReasonBox
            };

            foreach (var c in controls)
            {
                c.BorderBrush = Brushes.Gray;
                c.BorderThickness = new Thickness(1);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            Animal.Name = NameBox.Text.Trim();
            Animal.Species = SpeciesBox.Text.Trim();
            Animal.Gender = (GenderBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Animal.BirthDate = BirthDatePicker.SelectedDate!.Value;
            Animal.Weight = double.Parse(WeightBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            Animal.Height = double.Parse(HeightBox.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            Animal.Type = (TypeBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            Animal.ClimateZone = ClimateBox.Text.Trim();

            Animal.NeedsWarmShelter = WarmShelterBox.IsChecked == true;

            Animal.IsIsolated = IsolatedBox.IsChecked == true;
            Animal.IsolationReason = Animal.IsIsolated
                ? IsolationReasonBox.Text.Trim()
                : string.Empty;

            Animal.MotherId = MotherBox.SelectedValue?.ToString();
            Animal.FatherId = FatherBox.SelectedValue?.ToString();

            Animal.CageId = CageBox.SelectedValue?.ToString();

            DialogResult = true;
            Close();
        }

        private void IsolatedBox_Checked(object sender, RoutedEventArgs e)
        {
            IsolationReasonBox.IsEnabled = true;
        }

        private void IsolatedBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsolationReasonBox.IsEnabled = false;
            IsolationReasonBox.Text = "";
        }

        private void CageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CageWarning.Visibility = Visibility.Collapsed;

            if (CageBox.SelectedValue == null)
                return;

            string cageId = CageBox.SelectedValue.ToString();

            // Тимчасова тварина для перевірки
            var tempAnimal = new Animal
            {
                Species = SpeciesBox.Text.Trim(),
                Type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString(),
                NeedsWarmShelter = WarmShelterBox.IsChecked == true
            };

            if (!_cagesService.CanAssignAnimalSafe(cageId, tempAnimal))
            {
                CageWarning.Text = "⚠ This cage is incompatible with this animal";
                CageWarning.Visibility = Visibility.Visible;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
