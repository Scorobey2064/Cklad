using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cklad.Core.Models;

namespace Cklad.Dialogs
{
    public partial class InvoiceDialog : Window
    {
        public Invoice Invoice { get; }
        public List<Product> AvailableProducts { get; }
        public ObservableCollection<InvoiceItem> Items { get; }

        public InvoiceDialog(Invoice invoice, List<Product> availableProducts)
        {
            InitializeComponent();

            Invoice = invoice;
            AvailableProducts = availableProducts;
            Items = new ObservableCollection<InvoiceItem>(invoice.Items);

            if (Items.Count == 0)
                Items.Add(new InvoiceItem { Quantity = 1 });

            ItemsGrid.ItemsSource = Items;
            DataContext = this;
        }

        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            var item = new InvoiceItem { Quantity = 1 };

            if (AvailableProducts.Count == 1)
            {
                var p = AvailableProducts[0];
                item.ProductId = p.Id;
                item.ProductName = p.Name;
                item.Price = p.Price;
            }

            Items.Add(item);
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            var item = ItemsGrid.SelectedItem as InvoiceItem;
            if (item == null)
            {
                MessageBox.Show("Выберите строку для удаления.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Items.Remove(item);
        }

        private void ProductCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo == null) return;

            var product = combo.SelectedItem as Product;
            var item = combo.DataContext as InvoiceItem;

            if (product != null && item != null)
            {
                item.ProductId = product.Id;
                item.ProductName = product.Name;
                item.Price = product.Price;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (Items.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одну позицию в накладную!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var empty = Items.Where(i => i.ProductId == Guid.Empty).ToList();
            foreach (var item in empty)
                Items.Remove(item);

            if (Items.Count == 0)
            {
                MessageBox.Show("Выберите товар хотя бы в одной строке!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            foreach (var item in Items)
            {
                if (item.Quantity <= 0)
                {
                    MessageBox.Show($"Количество товара '{item.ProductName}' должно быть больше нуля!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            Invoice.Items.Clear();
            foreach (var item in Items)
                Invoice.Items.Add(item);

            Invoice.IsIncoming = TypeBox.SelectedIndex == 0;
            Invoice.Date = DateBox.SelectedDate ?? DateTime.Now;

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}