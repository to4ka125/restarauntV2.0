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
    /// Interaction logic for DrinksEdit.xaml
    /// </summary>
    public partial class DrinksEdit : Window
    {
        string fileName;
        public DrinksEdit()
        {
            InitializeComponent();
        }

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
                using (MySqlCommand cmd = new MySqlCommand($@"Update Menu Set name = '{name}',category_id = '{categoriesId}',description ='{description}',price='{price}' 
                                                              where menu_id = '{SafeData.drinks_id}'", con))
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Напиток '{name}' успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand($@"Select m.menu_id, m.name, concat(m.category_id,' ',c.name) ,m.description , m.price, m.Image From  Menu m 
                                                              Inner join Categories c  ON c.category_id = m.category_id
                                                             Where menu_id = '{SafeData.drinks_id}'",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    NameBox.Text = dt.Rows[0].ItemArray[1].ToString();
                    CategoriesBox.Text = dt.Rows[0].ItemArray[2].ToString();
                    DescriptionBox.Text = dt.Rows[0].ItemArray[3].ToString();
                    PriceBox.Text = dt.Rows[0].ItemArray[4].ToString();
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
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void DescriptionBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
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
                textBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void NameBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
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
                    MessageBox.Show($"Файл успешно сохранен в: {destinationPath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Btn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
