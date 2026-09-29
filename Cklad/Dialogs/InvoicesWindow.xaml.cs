using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Cklad.Core.Models;

namespace Cklad.Dialogs
{
    public partial class InvoicesWindow : Window
    {
        private readonly List<Invoice> _invoices;

        public event Action<Invoice> UnpostRequested;

        public InvoicesWindow(Warehouse warehouse, List<Invoice> invoices)
        {
            InitializeComponent();
            _invoices = invoices;

            HeaderText.Text = $"Накладные по складу: {warehouse.Name}";
            InvoicesGrid.ItemsSource = invoices;
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var invoice = InvoicesGrid.SelectedItem as Invoice;
            if (invoice == null)
            {
                MessageBox.Show("Выберите накладную из списка.",
                    "Просмотр", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Накладная: {invoice.Number}");
            sb.AppendLine($"Дата:      {invoice.Date:dd.MM.yyyy HH:mm}");
            sb.AppendLine($"Тип:       {invoice.TypeDisplay}");
            sb.AppendLine($"Статус:    {invoice.Status}");
            sb.AppendLine(new string('─', 60));
            sb.AppendLine(string.Format("{0,-25} {1,8} {2,12} {3,12}",
                "Товар", "Кол-во", "Цена", "Сумма"));

            foreach (var item in invoice.Items)
            {
                sb.AppendLine(string.Format("{0,-25} {1,8} {2,12:C} {3,12:C}",
                    item.ProductName, item.Quantity, item.Price, item.Quantity * item.Price));
            }

            sb.AppendLine(new string('─', 60));
            sb.AppendLine($"Итого: {invoice.TotalSum:C}");

            MessageBox.Show(sb.ToString(), "Детали накладной",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Unpost_Click(object sender, RoutedEventArgs e)
        {
            var invoice = InvoicesGrid.SelectedItem as Invoice;
            if (invoice == null)
            {
                MessageBox.Show("Выберите накладную из списка.",
                    "Отмена", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            UnpostRequested?.Invoke(invoice);
            InvoicesGrid.Items.Refresh();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
