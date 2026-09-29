using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Cklad.Core.Models;
using Cklad.Core.Services;

namespace Cklad.Core.ViewModels
{
    public class WarehouseViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IDialogService _dialogService;

        public ObservableCollection<Warehouse> Warehouses { get; }

        private Warehouse _selectedWarehouse;
        public Warehouse SelectedWarehouse
        {
            get => _selectedWarehouse;
            set
            {
                _selectedWarehouse = value;
                OnPropertyChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (SelectCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectCommand { get; }
        public ICommand BackCommand { get; }

        public WarehouseViewModel(MainViewModel mainViewModel, IDialogService dialogService)
        {
            _mainViewModel = mainViewModel;
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            var allWh = DataService.Instance.Warehouses;
            Warehouses = new ObservableCollection<Warehouse>(
                allWh.Where(w => w.OrganizationId == _mainViewModel.SelectedOrganization.Id));

            AddCommand = new RelayCommand(_ => Add());
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedWarehouse != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedWarehouse != null);
            SelectCommand = new RelayCommand(_ => Select(), _ => SelectedWarehouse != null);
            BackCommand = new RelayCommand(_ => _mainViewModel.NavigateToOrganizations());
        }

        public void Add()
        {
            var warehouse = new Warehouse
            {
                OrganizationId = _mainViewModel.SelectedOrganization.Id
            };

            if (_dialogService.ShowWarehouseDialog(warehouse))
            {
                DataService.Instance.Warehouses.Add(warehouse);
                Warehouses.Add(warehouse);
            }
        }

        public void Edit()
        {
            if (_dialogService.ShowWarehouseDialog(SelectedWarehouse))
            {
                OnPropertyChanged(nameof(Warehouses));
            }
        }

        public void Delete()
        {
            if (_dialogService.Confirm($"Удалить склад '{SelectedWarehouse.Name}'?", "Подтверждение"))
            {
                DataService.Instance.Warehouses.Remove(SelectedWarehouse);
                Warehouses.Remove(SelectedWarehouse);
            }
        }

        public void Select()
        {
            _mainViewModel.SelectedWarehouse = SelectedWarehouse;
            _mainViewModel.NavigateToProducts();
        }
    }
}
