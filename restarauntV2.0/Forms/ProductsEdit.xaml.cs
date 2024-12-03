using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace restarauntV2._0.Forms
{
    /// <summary>
    /// Interaction logic for ProductsEdit.xaml
    /// </summary>
    public partial class ProductsEdit : Window
    {
        public ProductsEdit()
        {
            InitializeComponent();
        }

        private void SafeProduct_Click(object sender, RoutedEventArgs e)
        {

            if (Quantity.Text == null || Quantity.Text.Trim() == "")
            {
                MessageBox.Show("Пожалуйста, укажите количество продуктов. Это поле не может быть пустым.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
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


            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($"Update products Set quantity = '{Quantity.Text.Replace(',', '.')}' Where product_id = '{SafeData.product_id}' ", con))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Количество продуктов успешно обновлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка при обновлении количества продуктов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        this.Close();
                    }
                }
            }
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
                using (MySqlCommand cmd = new MySqlCommand($@"SELECT 
                                                                p.name AS 'Product Name', 
                                                                p.quantity AS 'Quantity', 
                                                                p.unit_price AS 'Unit Price', 
                                                                CONCAT(p.supplier_id, '-', s.name) AS 'Supplier Info'
                                                                FROM 
                                                                    products p
                                                                INNER JOIN 
                                                                    supplier s ON s.supplier_id = p.supplier_id
                                                                WHERE 
                                                                    p.product_id ='{SafeData.product_id}'", con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    Name.Text = dt.Rows[0].ItemArray[0].ToString();
                    Quantity.Text = dt.Rows[0].ItemArray[1].ToString();
                    Unit_price.Text = dt.Rows[0].ItemArray[2].ToString();
                    Supplier.Text = dt.Rows[0].ItemArray[3].ToString();
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

    }
}



