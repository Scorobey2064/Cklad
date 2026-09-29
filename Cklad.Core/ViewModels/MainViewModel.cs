using System;
using System.Windows.Input;
using Cklad.Core.Models;
using Cklad.Core.Services;

namespace Cklad.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged(); }
        }

        private Organization _selectedOrganization;
        public Organization SelectedOrganization
        {
            get => _selectedOrganization;
            set { _selectedOrganization = value; OnPropertyChanged(); }
        }

        private Warehouse _selectedWarehouse;
        public Warehouse SelectedWarehouse
        {
            get => _selectedWarehouse;
            set { _selectedWarehouse = value; OnPropertyChanged(); }
        }

        public ICommand NavigateToOrganizationsCommand { get; }
        public ICommand NavigateToWarehousesCommand { get; }
        public ICommand NavigateToProductsCommand { get; }

        public MainViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            NavigateToOrganizationsCommand = new RelayCommand(_ => NavigateToOrganizations());
            NavigateToWarehousesCommand = new RelayCommand(_ => NavigateToWarehouses(),
                _ => SelectedOrganization != null);
            NavigateToProductsCommand = new RelayCommand(_ => NavigateToProducts(),
                _ => SelectedWarehouse != null);

            CurrentViewModel = new OrganizationViewModel(this, _dialogService);
        }

        public void NavigateToWarehouses()
        {
            if (SelectedOrganization != null)
                CurrentViewModel = new WarehouseViewModel(this, _dialogService);
        }

        public void NavigateToProducts()
        {
            if (SelectedWarehouse != null)
                CurrentViewModel = new ProductViewModel(this, _dialogService);
        }

        public void NavigateToOrganizations()
        {
            SelectedWarehouse = null;
            CurrentViewModel = new OrganizationViewModel(this, _dialogService);
        }
    }
}