using System.Collections.ObjectModel;
using Cklad.Core.Models;

namespace Cklad.Core.Services
{
    public class DataService
    {
        private static DataService _instance;

        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataService();
                }
                return _instance;
            }
        }

        public static void ResetInstance()
        {
            _instance = null;
        }

        public ObservableCollection<Organization> Organizations { get; } = new ObservableCollection<Organization>();
        public ObservableCollection<Warehouse> Warehouses { get; } = new ObservableCollection<Warehouse>();
        public ObservableCollection<Product> Products { get; } = new ObservableCollection<Product>();
        public ObservableCollection<Invoice> Invoices { get; } = new ObservableCollection<Invoice>();

        private DataService()
        {
            var org1 = new Organization { Name = "Три топора" };
            var org2 = new Organization { Name = "Боты для любой погоды" };
            Organizations.Add(org1);
            Organizations.Add(org2);

            var wh1 = new Warehouse
            {
                OrganizationId = org1.Id,
                Name = "Топоры №1",
                Address = "г. Москва, ул. Ленина, д. 1"
            };
            var wh2 = new Warehouse
            {
                OrganizationId = org1.Id,
                Name = "Топоры №2",
                Address = "г. Москва, ул. Мира, д. 5"
            };
            Warehouses.Add(wh1);
            Warehouses.Add(wh2);

            Products.Add(new Product
            {
                WarehouseId = wh1.Id,
                Name = "Топорища",
                Category = "Ручки",
                Manufacturer = "Завод по дерево обработке",
                Supplier = "Иванов",
                Price = 1000,
                Quantity = 50,
                DiscountPercent = 5
            });
        }
    }
}