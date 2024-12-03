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
    /// Interaction logic for DishesEdit.xaml
    /// </summary>
    public partial class DishesEdit : Window
    {
        string fileName;
        public DishesEdit()
        {
            InitializeComponent();
        }

           /*
        private void ComboBoxItem()
        {

        
            foreach (var child in ingredients.Children)
            {
                if (child is StackPanel stackPanel)
                {
                    ComboBox comboBox = stackPanel.Children.OfType<ComboBox>().FirstOrDefault();
                    TextBox textBox = stackPanel.Children.OfType<TextBox>().FirstOrDefault();

                    comboBox.SelectionChanged += (s, e) => { SelectionChanged(comboBox,textBox);};
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
           */

        private void SelectionChanged(ComboBox comboBox, TextBox textBox)
        {
            TextBlock textBlock = (TextBlock)comboBox.Template.FindName("textBlock", comboBox);
            if (comboBox.SelectedItem == null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                if(textBlock!=null){
                    textBlock.Visibility = Visibility.Collapsed;
                }
            }
              
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();

                /*
                int count = 0;
                using (MySqlCommand cmd = new MySqlCommand($"SELECT count(*) FROM restaurant.Menu_Ingredients where menu_id= '{SafeData.menuId}'",con))
                {
                    MySqlDataReader dr = cmd.ExecuteReader();
                   
                    while (dr.Read())
                    {
                        count = dr.GetInt32(0);
                    }

                    dr.Close();
                }
                */
                /*
                using (MySqlCommand cmd = new MySqlCommand($@"SELECT * FROM restaurant.Menu_Ingredients
                                                            where menu_id = '{SafeData.menuId}'",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    for (int i = 0; i < count; i++)
                    {
                        ComboBox comboBox = new ComboBox
                        {
                            Style = (Style)FindResource("ComboBox"),
                            SelectedIndex = int.Parse(dt.Rows[i].ItemArray[1].ToString()) - 1,
                            IsReadOnly = true,
                            IsDropDownOpen=false,
                            Width = 240,
                            Margin = new Thickness(0, 0, 20, 0)
                        };
                        comboBox.IsEnabledChanged += (s, args) =>
                        {
                            if (!comboBox.IsEnabled)
                                comboBox.IsDropDownOpen = false;
                        };

                        TextBox textBlock = new TextBox
                        {
                            Text = dt.Rows[i].ItemArray[2].ToString(),
                            IsReadOnly = true,
                            Style = (Style)FindResource("pcaholderText"),
                            FontSize = 12,
                            Padding = new Thickness(10),
                            Width = 80,
                            Height = 40,
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
                            MessageBox.Show("В блюде не может быть больше 20 ингредиентов");
                        }
                    }

                }
                */
                using (MySqlCommand cmd =  new MySqlCommand($@"Select * From menu Where menu_id = '{SafeData.menuId}'",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    NameBox.Text = dt.Rows[0].ItemArray[1].ToString();
                    CategoriesBox.SelectedIndex = int.Parse(dt.Rows[0].ItemArray[4].ToString()) - 1;
                    DescriptionBox.Text = dt.Rows[0].ItemArray[2].ToString();
                    PriceBox.Text = dt.Rows[0].ItemArray[3].ToString();
                    fileName = dt.Rows[0].ItemArray[5].ToString();

                    try
                    {
                        if (fileName != null || fileName != "")
                        {
                            image.Source = new BitmapImage(new Uri($"/Images/ImagesMenu/{dt.Rows[0]["Image"]}", UriKind.RelativeOrAbsolute));
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void PriceBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[^0-9.,]$"))
            {
                e.Handled = true;
            }
        }

            private void DescriptionBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        /*
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
         
        }
        */
        private void SafeDishes_Click(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == null || CategoriesBox.SelectedItem == null
               || DescriptionBox == null || PriceBox.Text == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (fileName == null)
            {
                MessageBox.Show("Пожалуйста, выберите фото для загрузки.", "Выбор фото", MessageBoxButton.OK, MessageBoxImage.Information);
                return;


            }
            string name = NameBox.Text;


            string[] categories = CategoriesBox.Text.Split(' ');
            string categoriesId = categories[0];

            string description = DescriptionBox.Text;



            if (Double.TryParse(PriceBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
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

            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();
                /*
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

                            if (Double.TryParse(textBox.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double Quantity))
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

                            if (Quantity > 0.5 )
                            {
                                MessageBox.Show("Не больше 500 гр на одно блюдо");
                                return;
                            }


                            using (MySqlCommand cmd = new MySqlCommand($@"Update Menu_Ingredients Set  quantity ='{Quantity.ToString().Replace(',','.')}'
                                                                          WHERE menu_id ='{SafeData.menuId}' and product_id ='{productId}'",con))
                            {
                                cmd.ExecuteNonQuery();
                            }
                   
                } */


                using (MySqlCommand cmd = new MySqlCommand($@"Update menu Set name = '{name}',description = '{description}',
                                                                           price = '{price}',category_id ='{categoriesId}',Image ='{fileName}' where menu_id ='{SafeData.menuId}'", con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
                MessageBox.Show($"Блюдо '{name}' успешно изменено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show($"Файл успешно сохранен в: {destinationPath}", "Успех");
                }
            }
        }

        private void CategoriesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)CategoriesBox.Template.FindName("textBlock", CategoriesBox);
            if (CategoriesBox.SelectedItem == null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                if (textBlock != null)
                {
                    textBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        /*
        private void DellIngredients_Click(object sender, RoutedEventArgs e)
        {
            if (ingredients.Children.Count > 2)
            {
                ingredients.Children.RemoveAt(ingredients.Children.Count - 1);

                CountIngredients.Text = ingredients.Children.Count.ToString();
            }
        }
        */
    }
}
