using System.Collections.Generic;
using Cklad.Core.Models;

namespace Cklad.Core.Services
{
    public interface IDialogService
    {
        string ShowInput(string title, string prompt, string defaultValue = "");
        bool Confirm(string message, string title);
        void ShowMessage(string message, string title, bool isError = false);
        string OpenFile(string filter, string title);

        bool ShowOrganizationDialog(Organization org);
        bool ShowProductDialog(Product product);
        bool ShowWarehouseDialog(Warehouse warehouse);
        bool ShowInvoiceDialog(Invoice invoice, List<Product> availableProducts);

        void ShowInvoicesListWindow(Warehouse warehouse, List<Invoice> invoices);
    }
}
