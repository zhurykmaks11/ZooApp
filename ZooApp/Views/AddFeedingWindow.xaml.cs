using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZooApp.Models;
using System.Windows.Input;

namespace ZooApp.Views
{
    public partial class AddFeedingWindow : Window
    {
        public FeedingSchedule Feeding { get; private set; }

        public AddFeedingWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 🧩 Перевірка на пусті поля
            if (string.IsNullOrWhiteSpace(AnimalNameBox.Text))
            {
                AnimalNameBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Animal name is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(FeedTypeBox.Text))
            {
                FeedTypeBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Feed type is required.");
                return;
            }

            if (!double.TryParse(QuantityBox.Text, out double qty) || qty <= 0 || qty > 1000)
            {
                QuantityBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Quantity must be between 0 and 1000 kg.");
                return;
            }

            if (!TimeSpan.TryParse(FeedingTimeBox.Text, out var time))
            {
                FeedingTimeBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Feeding time must be in format HH:mm (e.g., 10:30).");
                return;
            }

            var validSeasons = new[] { "зима", "весна", "літо", "осінь", "all year" };
            if (!validSeasons.Contains(SeasonBox.Text.Trim().ToLower()))
            {
                SeasonBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Season must be one of: зима, весна, літо, осінь, all year.");
                return;
            }

            Feeding = new FeedingSchedule
            {
                AnimalName = AnimalNameBox.Text.Trim(),
                FeedType = FeedTypeBox.Text.Trim(),
                QuantityKg = qty,
                FeedingTime = FeedingTimeBox.Text.Trim(),
                Season = SeasonBox.Text.Trim()
            };

            DialogResult = true;
            Close();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !double.TryParse(e.Text, out _);
        }
        
        private void QuantityBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
                e.Handled = true;
        }

    }
}