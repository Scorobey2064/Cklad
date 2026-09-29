using System.Collections.Generic;
using System.Linq;
using Cklad.Dialogs;
using Cklad.Core.Models;
using Cklad.Core.Services;

namespace Cklad.Services
{
    public class DialogService : IDialogService
    {

        public string ShowInput(string title, string prompt, string defaultValue = "")
        {
            return Microsoft.VisualBasic.Interaction.InputBox(prompt, title, defaultValue);
        }

        public bool Confirm(string message, string title)
        {
            return System.Windows.MessageBox.Show(message, title,
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes;
        }

        public void ShowMessage(string message, string title, bool isError = false)
        {
            System.Windows.MessageBox.Show(message, title,
                System.Windows.MessageBoxButton.OK,
                isError ? System.Windows.MessageBoxImage.Error : System.Windows.MessageBoxImage.Information);
        }

        public string OpenFile(string filter, string title)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog
            {
                Filter = filter,
                Title = title
            };
            return dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK ? dlg.FileName : null;
        }

        public bool ShowOrganizationDialog(Organization org)
        {
            var dialog = new OrganizationDialog
            {
                DataContext = org,
                Owner = System.Windows.Application.Current.MainWindow
            };
            return dialog.ShowDialog() == true;
        }

        public bool ShowProductDialog(Product product)
        {
            var dialog = new ProductDialog
            {
                DataContext = product,
                Owner = System.Windows.Application.Current.MainWindow
            };
            return dialog.ShowDialog() == true;
        }

        public bool ShowWarehouseDialog(Warehouse warehouse)
        {
            var dialog = new WarehouseDialog
            {
                DataContext = warehouse,
                Owner = System.Windows.Application.Current.MainWindow
            };
            return dialog.ShowDialog() == true;
        }

        public bool ShowInvoiceDialog(Invoice invoice, List<Product> availableProducts)
        {
            var dialog = new InvoiceDialog(invoice, availableProducts)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };
            return dialog.ShowDialog() == true;
        }

        public void ShowInvoicesListWindow(Warehouse warehouse, List<Invoice> invoices)
        {
            var window = new InvoicesWindow(warehouse, invoices)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            window.UnpostRequested += (invoice) =>
            {
                var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
                var mainVm = mainWindow?.DataContext as Cklad.Core.ViewModels.MainViewModel;

                if (mainVm?.CurrentViewModel is Cklad.Core.ViewModels.ProductViewModel productVm)
                {
                    productVm.UnpostInvoice(invoice);
                }
            };

            window.ShowDialog();
        }
    }
}