using System.Windows;

namespace Cklad.Dialogs
{
    public partial class OrganizationDialog : Window
    {
        public OrganizationDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                System.Windows.MessageBox.Show("Поле 'Наименование' не может быть пустым!",
                    "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                NameBox.Focus();
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
