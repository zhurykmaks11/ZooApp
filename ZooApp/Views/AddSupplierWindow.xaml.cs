using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ZooApp.Models;

namespace ZooApp.Views
{
    public partial class AddSupplierWindow : Window
    {
        public Supplier Supplier { get; private set; }

        public AddSupplierWindow()
        {
            InitializeComponent();
            Supplier = new Supplier();
        }

        public AddSupplierWindow(Supplier supplier) : this()
        {
            Supplier = supplier;

            NameBox.Text = supplier.Name;
            AddressBox.Text = supplier.Address;
            PhoneBox.Text = supplier.Phone;
            FeedBox.Text = string.Join(", ", supplier.FeedTypes);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Enter supplier name");
                return;
            }

            if (string.IsNullOrWhiteSpace(AddressBox.Text))
            {
                MessageBox.Show("Enter supplier address");
                return;
            }

            if (!Regex.IsMatch(PhoneBox.Text, @"^\+380\d{9}$"))
            {
                MessageBox.Show("Phone must be in +380XXXXXXXXX format");
                return;
            }

            if (string.IsNullOrWhiteSpace(FeedBox.Text))
            {
                MessageBox.Show("Enter at least one feed type");
                return;
            }

            Supplier.Name = NameBox.Text.Trim();
            Supplier.Address = AddressBox.Text.Trim();
            Supplier.Phone = PhoneBox.Text.Trim();
            Supplier.FeedTypes = FeedBox.Text
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();

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