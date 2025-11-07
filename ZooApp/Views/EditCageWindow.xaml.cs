using System.Linq;
using System.Windows;
using ZooApp.Services;
using ZooApp.Data;
using MongoDB.Bson;

namespace ZooApp.Views
{
    public partial class EditCageWindow : Window
    {
        private readonly CagesService _service;
        private readonly string _id;

        public EditCageWindow(string cageId)
        {
            InitializeComponent();
            _service = new CagesService(new MongoDbContext("mongodb://localhost:27017", "test"));
            _id = cageId;

            var cage = _service.GetCageById(cageId);
            LocationBox.Text = cage.Location;
            SizeBox.Text = cage.Size;
            CapacityBox.Text = cage.Capacity.ToString();
            SpeciesBox.Text = string.Join(", ", cage.CompatibleSpecies);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(CapacityBox.Text, out int capacity))
            {
                MessageBox.Show("Capacity must be number");
                return;
            }

            _service.UpdateCage(_id, LocationBox.Text.Trim(), SizeBox.Text.Trim(), capacity,
                SpeciesBox.Text.Split(',').Select(s => s.Trim().ToLower()).ToList());

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