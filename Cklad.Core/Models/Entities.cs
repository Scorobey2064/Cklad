using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cklad.Core.Models
{

    public class Organization
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }

    public class Warehouse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WarehouseId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public string Supplier { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public double DiscountPercent { get; set; }
        public string PhotoPath { get; set; }
    }

    public class Invoice
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Number { get; set; }
        public Guid WarehouseId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool IsIncoming { get; set; }
        public string Status { get; set; } = "Проведена";

        public ObservableCollection<InvoiceItem> Items { get; set; }
            = new ObservableCollection<InvoiceItem>();

        public string TypeDisplay => IsIncoming ? "Приход" : "Расход";

        public decimal TotalSum
        {
            get
            {
                if (Items == null) return 0;
                return Items.Sum(i => i.Quantity * i.Price);
            }
        }
    }

    public class InvoiceItem : INotifyPropertyChanged
    {
        private Guid _productId;
        public Guid ProductId
        {
            get => _productId;
            set { _productId = value; OnPropertyChanged(); }
        }

        private string _productName;
        public string ProductName
        {
            get => _productName;
            set { _productName = value; OnPropertyChanged(); }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); OnPropertyChanged(nameof(Total)); }
        }

        public decimal Total => Quantity * Price;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}