using Microsoft.Win32;
using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
    /// Interaction logic for DishesAdd.xaml
    /// </summary>
    public partial class DishesAdd : Window
    {
        string fileName;
        public DishesAdd()
        {
            InitializeComponent();
        }
        private void ComboBoxItem()
        {
            foreach (var child in ingredients.Children)
            {
                if (child is StackPanel stackPanel)
                {
                    ComboBox comboBox = stackPanel.Children.OfType<ComboBox>().FirstOrDefault();
                    TextBox textBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault();

                    comboBox.SelectionChanged += (s, e) => { SelectionChanged(comboBox, textBox); };
                    textBox.SelectionChanged += (s, e) => { };

                    using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
                    {
                        con.Open();

                        MySqlCommand cmd = new MySqlCommand("Select product_id, name From Products ", con);

                        MySqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            comboBox.Items.Add($"{dr.GetValue(0)}-{dr.GetValue(1)}");
                        }
                    }

                }
            }
        }
        private void SelectionChanged(ComboBox comboBox, TextBox textBox)

        {
            TextBlock textBlock = (TextBlock)comboBox.Template.FindName("textBlock", comboBox);
            if (comboBox.SelectedItem == null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            /*
            TextBlock textBlock = (TextBlock)comboBox.Template.FindName("textBlock", comboBox);
            textBlock.Visibility = Visibility.Collapsed;


              double totalPrice = 0;
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                try
                {
                    con.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return; // выход из метода если не удалось открыть соединение
                }

                MySqlCommand cmd = new MySqlCommand($"SELECT * FROM Products WHERE name = @name", con);
                cmd.Parameters.AddWithValue("@name", comboBox.SelectedItem.ToString());

                DataTable dt = new DataTable();
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                // Проверяем, есть ли данные
                if (dt.Rows.Count > 0)
                {
                    double priceUnit = Convert.ToDouble(dt.Rows[0].ItemArray[3]);

                    // Прибавляем к общей цене умножая цену на количество
                    if (double.TryParse(textBox.Text, out double quantity))
                    {
                        totalPrice += priceUnit * quantity;
                    }
                    else
                    {
                        MessageBox.Show("Введите корректное значение количества.");
                        return; // выходим, если значение количества некорректно
                    }
                }
                else
                {
                    MessageBox.Show("Продукт не найден.");
                    return; // выходим, если продукт не найден
                }


                PriceBox.Text = $"{totalPrice}";
            }
            */

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBoxItem();
        }

        private void DellIngredients_Click(object sender, RoutedEventArgs e)
        {
            if (ingredients.Children.Count > 2)
            {
                ingredients.Children.RemoveAt(ingredients.Children.Count - 1);

                CountIngredients.Text = ingredients.Children.Count.ToString();
            }
        }

        private void AddIngredients_Click(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = new ComboBox
            {
                Style = (Style)FindResource("ComboBox"),
                Width = 240,
                Tag = $" Ингредиент {ingredients.Children.Count + 1}",
                Margin = new Thickness(0, 0, 20, 0)
            };

            TextBox textBlock = new TextBox
            {
                Style = (Style)FindResource("pcaholderText"),
                FontSize = 12,
                Padding = new Thickness(10),
                Width = 80,
                Height = 40,
                Tag = "Кол-во",

            };

            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,

                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            if (ingredients.Children.Count != 20)
            {
                stackPanel.Children.Add(comboBox);
                stackPanel.Children.Add(textBlock);
                ingredients.Children.Add(stackPanel);
                CountIngredients.Text = ingredients.Children.Count.ToString();
                ComboBoxItem();
            }
            else
            {
                MessageBox.Show("В блюде не может быть больше 20 ингредиентов.",
                 "Ошибка",
                 MessageBoxButton.OK,
                 MessageBoxImage.Error);
            }
             
        }

        private void SafeDishes_Click(object sender, RoutedEventArgs e)
        {

            if (NameBox.Text == null || CategoriesBox.SelectedItem == null 
                || DescriptionBox==null || PriceBox.Text==null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (fileName ==null)
            {
                MessageBox.Show("Пожалуйста, выберите фото для загрузки.", "Выбор фото", MessageBoxButton.OK, MessageBoxImage.Information);
                return;            
            }

            string name = NameBox.Text;



            string[] categories = CategoriesBox.Text.Split(' ');
            string categoriesId = categories[0];

            string description = DescriptionBox.Text;

            if (Double.TryParse(PriceBox.Text.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
            {
                if (price < 150)
                {
                    MessageBox.Show("Цена блюда не может быть меньше  150 рублей . Пожалуйста, введите корректное значение.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Введите корректное числовое значение для цены блюда.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int maxId;
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();
               

                foreach (var child in ingredients.Children)
                    {
                        if (child is StackPanel stackPanel)
                        {
                            ComboBox comboBox = stackPanel.Children.OfType<ComboBox>().FirstOrDefault();
                            TextBox textBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault();
                            if (comboBox != null && textBox != null)
                            {
                                string[] ingredient = comboBox.Text.Split('-');
                                var productId = ingredient[0];
                            if (Double.TryParse(textBox.Text.Replace(',','.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double Quantity))
                            {
                                if (Quantity < 0)
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
                            if (Quantity > 0.5)
                                {
                                MessageBox.Show("Напоминание: Не добавляйте более 500 г продукта на одно блюдо. Это поможет сохранить качество!", "Совет", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                                }


                          
                            using (MySqlCommand cmd = new MySqlCommand("SELECT MAX(menu_id) FROM Menu;", con))
                            {
                                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                maxId = int.Parse(dt.Rows[0].ItemArray[0].ToString());
                            }
                            using (MySqlCommand cmd = new MySqlCommand($@"Insert into Menu_Ingredients 
                                                                              (menu_id,product_id,quantity) Values('{maxId+1}','{productId}','{Quantity.ToString().Replace(',','.')}')", con))
                                {
                                    cmd.ExecuteNonQuery();
                                }

                            }
                        }
                    }

                using (MySqlCommand cmd = new MySqlCommand($@"Insert into Menu (name,description,price,category_id,Image,terminalStatus) 
                                                              Values('{name}', '{description}','{price}','{categoriesId}','{fileName}', 'Показать')", con))
                {
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("🎉 Блюдо успешно добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Click_1(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                long fileSizeInBytes = fileInfo.Length;
                const long maxSizeInBytes = 2 * 1024 * 1024;

                if (fileSizeInBytes > maxSizeInBytes)
                {
                    MessageBox.Show("Размер файла превышает 2 МБ. Пожалуйста, выберите другой файл.", "Ошибка");
                }
                else
                {
                    fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                    string projectFolderPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                    string destinationFolderPath = System.IO.Path.Combine(projectFolderPath, "Images", "ImagesMenu");

                    if (!Directory.Exists(destinationFolderPath))
                    {
                        Directory.CreateDirectory(destinationFolderPath);
                    }
                    string destinationPath = System.IO.Path.Combine(destinationFolderPath, fileName);
                    try
                    {
                        File.Copy(openFileDialog.FileName, destinationPath, true);
                    }
                    catch (Exception ex)
                    {
                      
                    }
                   
                    image.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                }
            }
        }

        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void DescriptionBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void PriceBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[^0-9.,]$"))
            {
                e.Handled = true;
            }
        }

        private void CategoriesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)CategoriesBox.Template.FindName("textBlock",CategoriesBox);
            if (CategoriesBox.SelectedItem == null)
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
