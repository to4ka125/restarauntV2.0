using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Globalization;

namespace restarauntV2._0.Forms
{
    /// <summary>
    /// Interaction logic for ProductsAdd.xaml
    /// </summary>
    public partial class ProductsAdd : Window
    {
        public ProductsAdd()
        {
            InitializeComponent();
        }

        private void Supplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            TextBlock textBlock = (TextBlock)Supplier.Template.FindName("textBlock", Supplier);
            if (Supplier.Text == null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void SafeProduct_Click(object sender, RoutedEventArgs e)
        {

            if (Name.Text == null || Quantity.Text == null || Unit_price.Text == null || Supplier.SelectedItem==null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string input = Quantity.Text.Replace(',', '.');
            if (Double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double quantity))
            {
                if (quantity < 0)
                {
                    MessageBox.Show("Количество продуктов не может быть отрицательным. Пожалуйста, введите корректное значение.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Введите корректное числовое значение для количества продуктов.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            input = Unit_price.Text.Replace(',','.');
            if (Double.TryParse(input,NumberStyles.Any,CultureInfo.InvariantCulture,out double unitPrice))
            {
                if (unitPrice<0)
                {
                    MessageBox.Show("Стоимость ед продукта не может быть отрицательным. Пожалуйста, введите корректное значение.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Введите корректное числовое значение для стоимости за ед продукта.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string name = Name.Text;
            string [] supplier = Supplier.Text.Split('-');
            string supplierId = supplier[0];
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($@"Insert into products (name,quantity,unit_price,supplier_id) 
                                                              Values ('{name}','{quantity}','{unitPrice}','{supplierId}')",con))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Продукт успешно добавлен в вашу коллекцию!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            Name.Clear();
            Quantity.Clear();
            Unit_price.Clear();
            Supplier.SelectedItem =null;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand("Select supplier_id,name From supplier",con))
                {
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Supplier.Items.Add($"{dr.GetValue(0)}-{dr.GetValue(1)}");
                    }
                }
            }
        }

        private void Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void Quantity_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[^0-9.,]$"))
            {
                e.Handled = true;
            }
        }

        private void Unit_price_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[^0-9.,]$"))
            {
                e.Handled = true;
            }
        }
    }
}
