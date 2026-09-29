using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cklad.Core.Models;
using Cklad.Core.Services;
using Cklad.Core.ViewModels;

namespace Cklad.Tests
{
    [TestClass]
    public class ViewModelTests
    {
        private MockDialogService _mockDialog = new MockDialogService();

        [TestInitialize]
        public void Setup()
        {
            DataService.ResetInstance();
            var ds = DataService.Instance;
            ds.Organizations.Clear();
            ds.Warehouses.Clear();
            ds.Products.Clear();

            _mockDialog = new MockDialogService();

            var org = new Organization { Id = Guid.NewGuid(), Name = "Test Org" };
            ds.Organizations.Add(org);

            var wh = new Warehouse { Id = Guid.NewGuid(), OrganizationId = org.Id, Name = "Test Warehouse" };
            ds.Warehouses.Add(wh);

            ds.Products.Add(new Product
            {
                Id = Guid.NewGuid(),
                WarehouseId = wh.Id,
                Name = "Test Product",
                Price = 100,
                Quantity = 10
            });
        }


        [TestMethod]
        public void MainViewModel_InitialState_ShouldBeOrganizationViewModel()
        {
            var vm = new MainViewModel(_mockDialog);
            Assert.IsInstanceOfType(vm.CurrentViewModel, typeof(OrganizationViewModel));
        }

        [TestMethod]
        public void NavigateToWarehouses_WithSelectedOrg_ShouldChangeView()
        {
            var vm = new MainViewModel(_mockDialog);
            vm.SelectedOrganization = DataService.Instance.Organizations.First();
            vm.NavigateToWarehouses();
            Assert.IsInstanceOfType(vm.CurrentViewModel, typeof(WarehouseViewModel));
        }

        [TestMethod]
        public void NavigateToWarehouses_WithoutSelectedOrg_ShouldNotChangeView()
        {
            var vm = new MainViewModel(_mockDialog);
            var initial = vm.CurrentViewModel;
            vm.NavigateToWarehouses();
            Assert.AreEqual(initial, vm.CurrentViewModel);
        }


        [TestMethod]
        public void OrganizationViewModel_Add_ShouldAddNewOrg()
        {
            var mainVm = new MainViewModel(_mockDialog);
            var vm = new OrganizationViewModel(mainVm, _mockDialog);
            int before = DataService.Instance.Organizations.Count;

            vm.Add();

            Assert.AreEqual(before + 1, DataService.Instance.Organizations.Count);
            Assert.AreEqual("Mocked Org", DataService.Instance.Organizations.Last().Name);
        }

        [TestMethod]
        public void OrganizationViewModel_Delete_ShouldRemoveOrg()
        {
            var mainVm = new MainViewModel(_mockDialog);
            var vm = new OrganizationViewModel(mainVm, _mockDialog);
            vm.SelectedOrganization = DataService.Instance.Organizations.First();
            int before = DataService.Instance.Organizations.Count;

            vm.Delete();

            Assert.AreEqual(before - 1, DataService.Instance.Organizations.Count);
        }

        [TestMethod]
        public void OrganizationViewModel_DeleteCancelled_ShouldKeepOrg()
        {
            _mockDialog.ConfirmResult = false;
            var mainVm = new MainViewModel(_mockDialog);
            var vm = new OrganizationViewModel(mainVm, _mockDialog);
            vm.SelectedOrganization = DataService.Instance.Organizations.First();
            int before = DataService.Instance.Organizations.Count;

            vm.Delete();

            Assert.AreEqual(before, DataService.Instance.Organizations.Count);
        }

        [TestMethod]
        public void OrganizationViewModel_Select_ShouldNavigateToWarehouses()
        {
            var mainVm = new MainViewModel(_mockDialog);
            var vm = new OrganizationViewModel(mainVm, _mockDialog);
            vm.SelectedOrganization = DataService.Instance.Organizations.First();

            vm.Select();

            Assert.IsInstanceOfType(mainVm.CurrentViewModel, typeof(WarehouseViewModel));
        }


        [TestMethod]
        public void ProductViewModel_Add_ShouldAddNewProduct()
        {
            var mainVm = new MainViewModel(_mockDialog);
            mainVm.SelectedOrganization = DataService.Instance.Organizations.First();
            mainVm.SelectedWarehouse = DataService.Instance.Warehouses.First();
            var vm = new ProductViewModel(mainVm, _mockDialog);
            int before = vm.Products.Count;

            vm.Add();

            Assert.AreEqual(before + 1, vm.Products.Count);
        }

        [TestMethod]
        public void ProductViewModel_Delete_ShouldRemoveProduct()
        {
            var mainVm = new MainViewModel(_mockDialog);
            mainVm.SelectedOrganization = DataService.Instance.Organizations.First();
            mainVm.SelectedWarehouse = DataService.Instance.Warehouses.First();
            var vm = new ProductViewModel(mainVm, _mockDialog);
            vm.SelectedProduct = vm.Products.First();

            vm.Delete();

            Assert.AreEqual(0, vm.Products.Count);
        }


        [TestMethod]
        public void WarehouseViewModel_ShouldFilterByOrg()
        {
            var mainVm = new MainViewModel(_mockDialog);
            mainVm.SelectedOrganization = DataService.Instance.Organizations.First();

            DataService.Instance.Warehouses.Add(new Warehouse
            {
                OrganizationId = Guid.NewGuid(),
                Name = "Wrong Warehouse"
            });

            var vm = new WarehouseViewModel(mainVm, _mockDialog);

            Assert.AreEqual(1, vm.Warehouses.Count);
        }

        [TestMethod]
        public void ProductViewModel_ShouldFilterByWarehouse()
        {
            var mainVm = new MainViewModel(_mockDialog);
            mainVm.SelectedOrganization = DataService.Instance.Organizations.First();
            mainVm.SelectedWarehouse = DataService.Instance.Warehouses.First();

            DataService.Instance.Products.Add(new Product
            {
                WarehouseId = Guid.NewGuid(),
                Name = "Wrong Product"
            });

            var vm = new ProductViewModel(mainVm, _mockDialog);

            Assert.AreEqual(1, vm.Products.Count);
        }


        [TestMethod]
        public void DataService_ShouldInitializeTestData()
        {
            Assert.AreEqual(1, DataService.Instance.Organizations.Count);
            Assert.AreEqual(1, DataService.Instance.Warehouses.Count);
            Assert.AreEqual(1, DataService.Instance.Products.Count);
        }
    }
}
