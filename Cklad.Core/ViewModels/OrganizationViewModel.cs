using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Cklad.Core.Models;
using Cklad.Core.Services;

namespace Cklad.Core.ViewModels
{
    public class OrganizationViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IDialogService _dialogService;

        public ObservableCollection<Organization> Organizations => DataService.Instance.Organizations;

        private Organization _selectedOrganization;
        public Organization SelectedOrganization
        {
            get => _selectedOrganization;
            set
            {
                _selectedOrganization = value;
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

        public OrganizationViewModel(MainViewModel mainViewModel, IDialogService dialogService)
        {
            _mainViewModel = mainViewModel;
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            AddCommand = new RelayCommand(_ => Add());
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedOrganization != null);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => SelectedOrganization != null);
            SelectCommand = new RelayCommand(_ => Select(), _ => SelectedOrganization != null);
        }

        public void Add()
        {
            var org = new Organization();
            if (_dialogService.ShowOrganizationDialog(org))
            {
                Organizations.Add(org);
            }
        }

        public void Edit()
        {
            if (_dialogService.ShowOrganizationDialog(SelectedOrganization))
            {
                OnPropertyChanged(nameof(Organizations));
            }
        }

        public void Delete()
        {
            if (_dialogService.Confirm($"Удалить организацию '{SelectedOrganization.Name}'?", "Подтверждение"))
            {
                Organizations.Remove(SelectedOrganization);
            }
        }

        public void Select()
        {
            _mainViewModel.SelectedOrganization = SelectedOrganization;
            _mainViewModel.NavigateToWarehouses();
        }
    }
}