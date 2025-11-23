using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ZooApp.Services;
using ZooApp.Data;

namespace ZooApp.Views
{
    public partial class EditCageWindow : Window
    {
        private readonly CagesService _service;
        private readonly string _id;

        private readonly Regex sizeRegex =
            new Regex(@"^\s*\d{1,3}\s*[xX]\s*\d{1,3}\s*$");

        public EditCageWindow(string cageId)
        {
            InitializeComponent();
            _service = new CagesService(new MongoDbContext("mongodb://localhost:27017", "test"));
            _id = cageId;

            var cage = _service.GetCage(cageId);
            LocationBox.Text = cage.Location;
            SizeBox.Text = cage.Size;
            CapacityBox.Text = cage.Capacity.ToString();
            SpeciesBox.Text = string.Join(", ", cage.CompatibleSpecies);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // VALIDATE SIZE
            if (!sizeRegex.IsMatch(SizeBox.Text))
            {
                MessageBox.Show("❌ Size must be in format NNxNN (example: 50x30)", "Error");
                return;
            }

            if (!int.TryParse(CapacityBox.Text, out int capacity) || capacity <= 0)
            {
                MessageBox.Show("Capacity must be a positive number!");
                return;
            }

            _service.UpdateCage(
                _id,
                LocationBox.Text.Trim(),
                SizeBox.Text.Trim(),
                capacity,
                SpeciesBox.Text.Split(',')
                    .Select(s => s.Trim().ToLower())
                    .ToList()
            );

            MessageBox.Show("✅ Cage updated!");
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}