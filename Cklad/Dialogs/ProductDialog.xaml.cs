using System.Globalization;
using System.Windows;
using Cklad.Core.Models;      

namespace Cklad.Dialogs
{
    public partial class ProductDialog : Window
    {
        public ProductDialog()
        {
            InitializeComponent();
        }

        private void SelectPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var product = DataContext as Product;
                if (product != null)
                {
                    product.PhotoPath = dlg.FileName;
                    var temp = DataContext;
                    DataContext = null;
                    DataContext = temp;
                }
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var product = DataContext as Product;
            if (product == null) { DialogResult = false; return; }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                System.Windows.MessageBox.Show("Поле 'Наименование' не может быть пустым!",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                NameBox.Focus();
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                System.Windows.MessageBox.Show("Поле 'Цена' должно быть положительным числом!",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                PriceBox.Focus();
                return;
            }
            product.Price = price;

            if (!int.TryParse(QuantityBox.Text, out int qty) || qty < 0)
            {
                System.Windows.MessageBox.Show("Поле 'Остаток' должно быть целым неотрицательным числом!",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                QuantityBox.Focus();
                return;
            }
            product.Quantity = qty;

            if (!double.TryParse(DiscountBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double disc) || disc < 0 || disc > 100)
            {
                System.Windows.MessageBox.Show("Скидка должна быть числом от 0 до 100!",
                    "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                DiscountBox.Focus();
                return;
            }
            product.DiscountPercent = disc;

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}