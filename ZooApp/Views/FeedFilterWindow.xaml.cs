using System.Windows;
using System.Windows.Controls;

namespace ZooApp.Views
{
    public partial class FeedFilterWindow : Window
    {
        public string FeedType { get; private set; }
        public string Season { get; private set; }

        public FeedFilterWindow()
        {
            InitializeComponent();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            FeedType = FeedBox.Text.Trim();
            Season = (SeasonBox.SelectedItem as ComboBoxItem)?.Content.ToString();
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