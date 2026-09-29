using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cklad.Core.Models;
using Cklad.Core.Services;
using Cklad.Core.ViewModels;

namespace Cklad.Tests
{
    [TestClass]
    public class WarehouseViewModelTests
    {
        private MockDialogService _mockDialog;
        private MainViewModel _mainVm;

        [TestInitialize]
        public void Setup()
        {
            DataService.ResetInstance();
            var ds = DataService.Instance;
            ds.Organizations.Clear();
            ds.Warehouses.Clear();
            ds.Products.Clear();
            ds.Invoices.Clear();

            var org = new Organization { Id = Guid.NewGuid(), Name = "Test Org" };
            ds.Organizations.Add(org);

            ds.Warehouses.Add(new Warehouse
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                Name = "Existing Warehouse",
                Address = "Test Address"
            });

            _mockDialog = new MockDialogService();
            _mainVm = new MainViewModel(_mockDialog);
            _mainVm.SelectedOrganization = ds.Organizations.First();
        }

        [TestMethod]
        public void Add_WithValidName_ShouldAddWarehouse()
        {
            _mockDialog.WarehouseDialogResult = true;
            _mockDialog.WarehouseName = "New Warehouse";
            _mockDialog.WarehouseAddress = "New Address";

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);
            int before = vm.Warehouses.Count;

            vm.Add();

            Assert.AreEqual(before + 1, vm.Warehouses.Count,
                "Количество складов должно увеличиться на 1.");

            var added = vm.Warehouses.Last();
            Assert.AreEqual("New Warehouse", added.Name);
            Assert.AreEqual("New Address", added.Address);
        }

        [TestMethod]
        public void Add_WithCancelledDialog_ShouldNotAddWarehouse()
        {
            _mockDialog.WarehouseDialogResult = false;

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);
            int before = vm.Warehouses.Count;

            vm.Add();

            Assert.AreEqual(before, vm.Warehouses.Count,
                "Склад не должен добавляться, если диалог отменён.");
        }

        [TestMethod]
        public void Add_ShouldAssignCorrectOrganizationId()
        {
            _mockDialog.WarehouseDialogResult = true;
            _mockDialog.WarehouseName = "New Warehouse";
            _mockDialog.WarehouseAddress = "New Address";

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);
            vm.Add();

            var added = vm.Warehouses.Last();
            Assert.AreEqual(_mainVm.SelectedOrganization.Id, added.OrganizationId,
                "Новый склад должен быть привязан к выбранной организации.");
        }

        [TestMethod]
        public void Edit_WithValidName_ShouldChangeName()
        {
            _mockDialog.WarehouseDialogResult = true;
            _mockDialog.WarehouseName = "Renamed Warehouse";
            _mockDialog.WarehouseAddress = "Renamed Address";

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);
            vm.SelectedWarehouse = vm.Warehouses.First();

            vm.Edit();

            Assert.AreEqual("Renamed Warehouse", vm.SelectedWarehouse.Name);
            Assert.AreEqual("Renamed Address", vm.SelectedWarehouse.Address);
        }

        [TestMethod]
        public void Edit_WithCancelledDialog_ShouldKeepOldName()
        {
            _mockDialog.WarehouseDialogResult = false;

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);
            vm.SelectedWarehouse = vm.Warehouses.First();
            string originalName = vm.SelectedWarehouse.Name;
            string originalAddress = vm.SelectedWarehouse.Address;

            vm.Edit();

            Assert.AreEqual(originalName, vm.SelectedWarehouse.Name);
            Assert.AreEqual(originalAddress, vm.SelectedWarehouse.Address);
        }

        [TestMethod]
        public void Delete_WithConfirmation_ShouldRemoveWarehouse()
        {
            _mockDialog.ConfirmResult = true;

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);
            vm.SelectedWarehouse = vm.Warehouses.First();
            int before = vm.Warehouses.Count;

            vm.Delete();

            Assert.AreEqual(before - 1, vm.Warehouses.Count);
        }

        [TestMethod]
        public void Delete_WithCancellation_ShouldKeepWarehouse()
        {
            _mockDialog.ConfirmResult = false;

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);
            vm.SelectedWarehouse = vm.Warehouses.First();
            int before = vm.Warehouses.Count;

            vm.Delete();

            Assert.AreEqual(before, vm.Warehouses.Count);
        }

        [TestMethod]
        public void Select_ShouldNavigateToProducts()
        {
            var vm = new WarehouseViewModel(_mainVm, _mockDialog);
            vm.SelectedWarehouse = vm.Warehouses.First();

            vm.Select();

            Assert.AreEqual(vm.SelectedWarehouse, _mainVm.SelectedWarehouse);
            Assert.IsInstanceOfType(_mainVm.CurrentViewModel, typeof(ProductViewModel));
        }

        [TestMethod]
        public void Back_ShouldNavigateToOrganizations()
        {
            var vm = new WarehouseViewModel(_mainVm, _mockDialog);

            vm.BackCommand.Execute(null);

            Assert.IsInstanceOfType(_mainVm.CurrentViewModel, typeof(OrganizationViewModel));
        }

        [TestMethod]
        public void WarehouseViewModel_ShouldFilterWarehousesByOrganization()
        {
            DataService.Instance.Warehouses.Add(new Warehouse
            {
                OrganizationId = Guid.NewGuid(),
                Name = "Foreign Warehouse",
                Address = "Foreign Address"
            });

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);

            Assert.AreEqual(1, vm.Warehouses.Count);
            Assert.AreEqual("Existing Warehouse", vm.Warehouses.First().Name);
        }

        [TestMethod]
        public void Warehouses_ShouldBeEmpty_WhenOrganizationHasNoWarehouses()
        {
            var emptyOrg = new Organization { Id = Guid.NewGuid(), Name = "Empty Org" };
            DataService.Instance.Organizations.Add(emptyOrg);

            _mainVm.SelectedOrganization = emptyOrg;

            var vm = new WarehouseViewModel(_mainVm, _mockDialog);

            Assert.AreEqual(0, vm.Warehouses.Count,
                "У организации без складов список должен быть пустым.");
        }
    }
}