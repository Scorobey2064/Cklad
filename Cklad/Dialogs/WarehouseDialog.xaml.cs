using System.Windows;

namespace Cklad.Dialogs
{
    public partial class WarehouseDialog : Window
    {
        public WarehouseDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Поле 'Название склада' не может быть пустым!",
                    "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                NameBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(AddressBox.Text))
            {
                MessageBox.Show("Поле 'Адрес' не может быть пустым!",
                    "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                AddressBox.Focus();
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
