using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Cklad.Core.Models;
using Cklad.Core.Services;

namespace Cklad.Core.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IDialogService _dialogService;

        public ObservableCollection<Product> Products { get; }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand InvoiceInCommand { get; }
        public ICommand InvoiceOutCommand { get; }
        public ICommand ShowInvoicesCommand { get; }

        public ProductViewModel(MainViewModel mainViewModel, IDialogService dialogService)
        {
            _mainViewModel = mainViewModel;
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            var all = DataService.Instance.Products;
            Products = new ObservableCollection<Product>(
                all.Where(p => p.WarehouseId == _mainViewModel.SelectedWarehouse.Id));

            AddCommand = new RelayCommand(_ => Add());
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedProduct != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedProduct != null);
            ImportCommand = new RelayCommand(_ => Import());
            BackCommand = new RelayCommand(_ => _mainViewModel.NavigateToWarehouses());
            InvoiceInCommand = new RelayCommand(_ => CreateInvoice(true));
            InvoiceOutCommand = new RelayCommand(_ => CreateInvoice(false));
            ShowInvoicesCommand = new RelayCommand(_ => ShowInvoicesList());
        }

        public void Add()
        {
            var product = new Product { WarehouseId = _mainViewModel.SelectedWarehouse.Id };
            if (_dialogService.ShowProductDialog(product))
            {
                DataService.Instance.Products.Add(product);
                Products.Add(product);
            }
        }

        public void Edit()
        {
            if (_dialogService.ShowProductDialog(SelectedProduct))
            {
                OnPropertyChanged(nameof(Products));
            }
        }

        public void Delete()
        {
            if (_dialogService.Confirm($"Удалить товар '{SelectedProduct.Name}'?", "Подтверждение"))
            {
                DataService.Instance.Products.Remove(SelectedProduct);
                Products.Remove(SelectedProduct);
            }
        }

        public void CreateInvoice(bool isIncoming)
        {
            var invoice = new Invoice
            {
                WarehouseId = _mainViewModel.SelectedWarehouse.Id,
                IsIncoming = isIncoming,
                Date = DateTime.Now,
                Status = "Проведена",
                Number = $"Н-{(DataService.Instance.Invoices.Count + 1):D4}"
            };

            var availableProducts = Products.ToList();
            if (availableProducts.Count == 0)
            {
                _dialogService.ShowMessage("На складе нет товаров!", "Ошибка", true);
                return;
            }

            if (!_dialogService.ShowInvoiceDialog(invoice, availableProducts))
                return;

            if (!isIncoming)
            {
                foreach (var item in invoice.Items)
                {
                    var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null) continue;

                    if (product.Quantity < item.Quantity)
                    {
                        _dialogService.ShowMessage(
                            $"Недостаточно товара '{product.Name}'! Доступно: {product.Quantity}, запрошено: {item.Quantity}",
                            "Ошибка", true);
                        return;
                    }
                }
            }

            foreach (var item in invoice.Items)
            {
                var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) continue;

                product.Quantity += isIncoming ? item.Quantity : -item.Quantity;
            }
            OnPropertyChanged(nameof(Products));

            DataService.Instance.Invoices.Add(invoice);

            string type = isIncoming ? "Приход" : "Расход";
            _dialogService.ShowMessage(
                $"Накладная {invoice.Number} ({type}) успешно проведена!",
                "Готово");
        }

        public void ShowInvoicesList()
        {
            var invoices = DataService.Instance.Invoices
                .Where(i => i.WarehouseId == _mainViewModel.SelectedWarehouse.Id)
                .OrderByDescending(i => i.Date)
                .ToList();

            if (invoices.Count == 0)
            {
                _dialogService.ShowMessage(
                    "По этому складу ещё не оформлялось ни одной накладной.",
                    "Накладные");
                return;
            }

            _dialogService.ShowInvoicesListWindow(_mainViewModel.SelectedWarehouse, invoices);
        }

        public void UnpostInvoice(Invoice invoice)
        {
            if (invoice == null) return;

            if (invoice.Status == "Отменена")
            {
                _dialogService.ShowMessage("Накладная уже отменена.", "Информация");
                return;
            }

            if (!_dialogService.Confirm(
                $"Отменить проведение накладной {invoice.Number}?\n" +
                "Остатки товаров будут возвращены к исходным значениям.",
                "Отмена проведения"))
                return;

            foreach (var item in invoice.Items)
            {
                var product = Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null) continue;

                if (invoice.IsIncoming)
                {
                    product.Quantity -= item.Quantity;
                    if (product.Quantity < 0) product.Quantity = 0;
                }
                else
                {
                    product.Quantity += item.Quantity;
                }
            }

            invoice.Status = "Отменена";
            OnPropertyChanged(nameof(Products));

            _dialogService.ShowMessage(
                $"Накладная {invoice.Number} отменена. Остатки восстановлены.",
                "Готово");
        }

        public void Import()
        {
            string path = _dialogService.OpenFile(
                "Таблицы (*.csv;*.xlsx)|*.csv;*.xlsx|CSV (*.csv)|*.csv|Excel (*.xlsx)|*.xlsx",
                "Выберите файл для импорта");

            if (string.IsNullOrEmpty(path)) return;

            try
            {
                int count = path.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase)
                    ? ImportFromExcel(path)
                    : ImportFromCsv(path);

                _dialogService.ShowMessage($"Успешно импортировано: {count}", "Импорт завершён");
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Ошибка при чтении файла:\n{ex.Message}", "Ошибка импорта", true);
            }
        }

        private int ImportFromCsv(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int count = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] parts = line.Split(';');
                if (parts.Length < 7) continue;

                AddProductFromParts(parts);
                count++;
            }
            return count;
        }

        private int ImportFromExcel(string path)
        {
            int count = 0;
            using (var wb = new ClosedXML.Excel.XLWorkbook(path))
            {
                var ws = wb.Worksheet(1);
                var rows = ws.RangeUsed().RowsUsed();
                bool firstRow = true;

                foreach (var row in rows)
                {
                    if (firstRow) { firstRow = false; continue; }

                    string[] parts = new string[7];
                    for (int c = 1; c <= 7; c++)
                    {
                        parts[c - 1] = row.Cell(c).GetString();
                    }

                    AddProductFromParts(parts);
                    count++;
                }
            }
            return count;
        }

        private void AddProductFromParts(string[] parts)
        {
            decimal.TryParse(parts[4].Trim().Replace(',', '.'), NumberStyles.Any,
                CultureInfo.InvariantCulture, out decimal price);
            int.TryParse(parts[5].Trim(), out int qty);
            double.TryParse(parts[6].Trim().Replace(',', '.'), NumberStyles.Any,
                CultureInfo.InvariantCulture, out double disc);

            if (price < 0 || qty < 0 || disc < 0 || disc > 100) return;
            if (string.IsNullOrWhiteSpace(parts[0])) return;

            var product = new Product
            {
                WarehouseId = _mainViewModel.SelectedWarehouse.Id,
                Name = parts[0].Trim(),
                Category = parts[1].Trim(),
                Manufacturer = parts[2].Trim(),
                Supplier = parts[3].Trim(),
                Price = price,
                Quantity = qty,
                DiscountPercent = disc
            };

            DataService.Instance.Products.Add(product);
            Products.Add(product);
        }
    }
}
