using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZooApp.Services;
using ZooApp.Data;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class MedicalHistoryWindow : Window
    {
        private readonly MedicalService _service;

        public MedicalHistoryWindow(string recordId)
        {
            InitializeComponent();

            var context = new MongoDbContext("mongodb://localhost:27017", "test");
            _service = new MedicalService(context);

            LoadHistory(recordId);
        }

        private void LoadHistory(string recordId)
        {
            var record = _service.GetRecord(recordId);
            if (record == null)
            {
                MessageBox.Show("Medical record not found.", "Error");
                return;
            }

            var checkups = record.Checkups
                .OrderByDescending(c => c.Date)
                .ToList();

            foreach (var c in checkups)
            {
                var border = new Border
                {
                    BorderBrush = System.Windows.Media.Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var panel = new StackPanel();

                panel.Children.Add(new TextBlock
                {
                    Text = "📅 Date: " + c.Date.ToString("dd.MM.yyyy"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                });

                panel.Children.Add(new TextBlock
                {
                    Text = "Weight: " + c.Weight + " kg"
                });

                panel.Children.Add(new TextBlock
                {
                    Text = "Height: " + c.Height + " m"
                });

                // Vaccinations
                string vacc = c.Vaccinations != null && c.Vaccinations.Any()
                    ? string.Join(", ", c.Vaccinations)
                    : "—";

                panel.Children.Add(new TextBlock
                {
                    Text = "Vaccinations: " + vacc
                });

                // Illnesses
                string ill = c.Illnesses != null && c.Illnesses.Any()
                    ? string.Join(", ", c.Illnesses)
                    : "—";

                panel.Children.Add(new TextBlock
                {
                    Text = "Illnesses: " + ill
                });

                // Treatment
                panel.Children.Add(new TextBlock
                {
                    Text = "Treatment: " + (string.IsNullOrWhiteSpace(c.Treatment) ? "—" : c.Treatment)
                });

                border.Child = panel;
                HistoryPanel.Children.Add(border);
            }
        }
    }
}
