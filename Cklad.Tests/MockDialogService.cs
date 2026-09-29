using System.Collections.Generic;
using Cklad.Core.Models;
using Cklad.Core.Services;

namespace Cklad.Tests
{
    public class MockDialogService : IDialogService
    {
        public string InputResult { get; set; } = "Test Value";
        public bool ConfirmResult { get; set; } = true;

        public bool OrganizationDialogResult { get; set; } = true;
        public bool ProductDialogResult { get; set; } = true;
        public bool WarehouseDialogResult { get; set; } = true;
        public bool InvoiceDialogResult { get; set; } = true;

        public string WarehouseName { get; set; } = "Mocked Warehouse";
        public string WarehouseAddress { get; set; } = "Mocked Address";

        public List<InvoiceItem> InvoiceItemsToAdd { get; set; } = new List<InvoiceItem>();

        public bool ShowInvoicesListWindowCalled { get; private set; }

        public string ShowInput(string title, string prompt, string defaultValue = "")
            => InputResult;

        public bool Confirm(string message, string title) => ConfirmResult;

        public void ShowMessage(string message, string title, bool isError = false) { }

        public string OpenFile(string filter, string title) => string.Empty;

        public bool ShowOrganizationDialog(Organization org)
        {
            if (OrganizationDialogResult) org.Name = "Mocked Org";
            return OrganizationDialogResult;
        }

        public bool ShowProductDialog(Product product)
        {
            if (ProductDialogResult) product.Name = "Mocked Product";
            return ProductDialogResult;
        }

        public bool ShowWarehouseDialog(Warehouse warehouse)
        {
            if (WarehouseDialogResult)
            {
                warehouse.Name = WarehouseName;
                warehouse.Address = WarehouseAddress;
            }
            return WarehouseDialogResult;
        }

        public bool ShowInvoiceDialog(Invoice invoice, List<Product> availableProducts)
        {
            if (InvoiceDialogResult)
            {
                foreach (var item in InvoiceItemsToAdd)
                    invoice.Items.Add(item);
            }
            return InvoiceDialogResult;
        }

        public void ShowInvoicesListWindow(Warehouse warehouse, List<Invoice> invoices)
        {
            ShowInvoicesListWindowCalled = true;
        }
    }
}
