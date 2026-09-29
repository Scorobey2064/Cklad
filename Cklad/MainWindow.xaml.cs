using System;
using System.Windows;
using Cklad.Core.Services;      
using Cklad.Core.ViewModels;    
using Cklad.Services;           

namespace Cklad
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                IDialogService dialogService = new DialogService();
                DataContext = new MainViewModel(dialogService);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка инициализации:\n\n" +
                    $"Сообщение: {ex.Message}\n\n" +
                    $"Внутренняя ошибка: {ex.InnerException?.Message}",
                    "Диагностика",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}