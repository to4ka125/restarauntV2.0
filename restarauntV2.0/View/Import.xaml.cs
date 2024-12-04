using Microsoft.Win32;
using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace restarauntV2._0.View
{
    /// <summary>
    /// Interaction logic for Import.xaml
    /// </summary>
    public partial class Import : UserControl
    {
        public Import()
        {
            InitializeComponent();
        }
        string fileName;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы csv| *.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                FileName.Text = System.IO.Path.GetFileName(openFileDialog.FileName);
                fileName = fileInfo.ToString();
            }
            else
            {
                fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string[] readText = File.ReadAllLines(fileName, Encoding.GetEncoding(1251));
            string[] titleField = readText[0].Split(';').Select(field => field.Trim().Trim('"')).ToArray();
            string tableName = tablesName.Text;
            string[] valField;
            string errorMsg = string.Empty;
            string query = string.Empty;
            int count = 0;
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
               
                con.Open();

                string[] dbHeaders = GetDataBaseHeaderCheck(con, tableName);
                con.Close();

                if (titleField.SequenceEqual(dbHeaders))
                {
                    con.Open();
                    foreach (string str in readText.Skip(1).ToArray())
                    {
                        valField = str.Split(';').Select(field => field.Trim().Trim('"')).ToArray();
                        switch (tableName)
                        {
                            case "users":
                                query = $@"Insert into `users`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}','{valField[6]}','{valField[7]}')";
                                break;

                            case "tables":
                                query = $@"Insert into `tables`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}','{valField[3]}')";
                                break;

                            case "supplier":
                                query = $@"Insert into `supplier` ({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}')";
                                break;

                            case "reservations":
                                query = $@"Insert into `reservations`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}','{valField[3]}','{valField[4]}')";
                                break;

                            case "products":
                                var supplierkQuery = $"SELECT COUNT(*) FROM supplier WHERE supplier_id = '{valField[4]}'";

                                using (MySqlCommand supplierCheck = new MySqlCommand(supplierkQuery, con))
                                {
                                    count = int.Parse(supplierCheck.ExecuteScalar().ToString());
                                    if (count == 0)
                                    {
                                        MessageBox.Show("Сначала заполните таблицу поставщики");
                                        return;
                                    }
                                }

                                query = $@"Insert into `products`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}','{valField[3]}','{valField[4]}')";
                                break;

                            case "orders":
                                var ordersCheckQuery = $"SELECT COUNT(*) FROM users WHERE user_id = '{valField[1]}'";

                                using (MySqlCommand orderCheck = new MySqlCommand(ordersCheckQuery, con))
                                {
                                    count = int.Parse(orderCheck.ExecuteScalar().ToString());
                                    if (count == 0)
                                    {
                                        MessageBox.Show("Сначала заполните таблицу пользователи");
                                        return;
                                    }
                                }

                                query = $@"Insert into `orders`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}','{valField[3]}','{valField[4]}','{valField[4]}','{valField[5]}')";
                                break;

                            case "order_items":
                                var menuOrderCheckQuery = $"SELECT COUNT(*) FROM menu WHERE menu_id = '{valField[1]}'";
                                var orderCheckQuery = $"SELECT COUNT(*) FROM orders WHERE order_id = '{valField[0]}'";

                                using (MySqlCommand menuCheck = new MySqlCommand(menuOrderCheckQuery, con))
                                {
                                    count = int.Parse(menuCheck.ExecuteScalar().ToString());
                                    if (count == 0)
                                    {
                                        MessageBox.Show("Сначала заполните таблицу меню");
                                        return;
                                    }
                                }
                                var orderCount = 0;
                                using (MySqlCommand orderCheck = new MySqlCommand(orderCheckQuery, con))
                                {
                                    orderCount = int.Parse(orderCheck.ExecuteScalar().ToString());
                                    if (orderCount == 0)
                                    {
                                        MessageBox.Show("Сначала заполните таблицу заказы");
                                        return;
                                    }
                                }

                                query = $@"Insert into `orders`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}')";
                                break;

                            case "menu_ingredients":

                                var menuCheckQuery = $"SELECT COUNT(*) FROM menu WHERE menu_id = '{valField[0]}'";
                                var productCheckQuery = $"SELECT COUNT(*) FROM products WHERE product_id = '{valField[1]}'";

                                using (MySqlCommand menuCheck = new MySqlCommand(menuCheckQuery, con))
                                {
                                    count = int.Parse(menuCheck.ExecuteScalar().ToString());
                                    if (count == 0)
                                    {
                                        MessageBox.Show("Сначала заполните таблицу меню");
                                        return;
                                    }
                                }

                                var countProduct = 0;
                                using (MySqlCommand productCheck = new MySqlCommand(productCheckQuery, con))
                                {
                                    countProduct = int.Parse(productCheck.ExecuteScalar().ToString());
                                    if (countProduct == 0)
                                    {
                                        MessageBox.Show("Сначала заполните таблицу продукты");
                                        return;
                                    }
                                }
                                query = $@"Insert into `orders`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}')";
                                break;


                            case "menu":
                                var categoriesCheckQuery = $"SELECT COUNT(*) FROM categories WHERE category_id = '{valField[4]}'";

                                using (MySqlCommand categoriesCheck = new MySqlCommand(categoriesCheckQuery, con))
                                {
                                    count = int.Parse(categoriesCheck.ExecuteScalar().ToString());
                                    if (count == 0)
                                    {
                                        MessageBox.Show("Сначала заполните таблицу категории");
                                        return;
                                    }
                                }

                                query = $@"Insert into `menu`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}','{valField[6]}')";
                                break;

                            case "customers":
                                query = $@"Insert into `customers`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}',
                                            '{valField[2]}','{valField[3]}','{valField[4]}','{valField[5]}')";

                                break;
                            case "categories":
                                query = $@"Insert into `categories`({string.Join(",", titleField)}) Values ('{valField[0]}','{valField[1]}'
                                          )";
                                break;
                        }

                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                errorMsg = ex.Message;
                            }



                        }
                    }

                    MessageBox.Show("Данные импортированны");

                    tablesName.SelectedItem = null;
                    
                    if (errorMsg.Length > 0) MessageBox.Show(errorMsg);

                    else
                    {
                        MessageBox.Show("Ошибка импортирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                try
                {
                    con.Open();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message,"Ошибка",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
               
                using (MySqlCommand cmd = new MySqlCommand($@"SHOW TABLES", con))
                {
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        tablesName.Items.Add(dr.GetValue(0));
                    }
                }
            }
        }

        private string[] GetDataBaseHeaderCheck(MySqlConnection con, string tableName)
        {
            using (MySqlCommand cmd = new MySqlCommand($"Select * From {tableName} Limit 0", con))
            {
                MySqlDataReader dr = cmd.ExecuteReader();
                var columnNames = Enumerable.Range(0, dr.FieldCount)
                                         .Select(dr.GetName)
                                         .ToArray();
                return columnNames;
            }


        }

        private void tablesName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)tablesName.Template.FindName("textBlock",tablesName);

            if (tablesName.SelectedItem==null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
        }
    }
}
