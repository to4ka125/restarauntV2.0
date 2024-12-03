using MySql.Data.MySqlClient;
using restarauntV2._0.Forms;
using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace restarauntV2._0.View
{
    /// <summary>
    /// Interaction logic for Dishes.xaml
    /// </summary>
    public partial class Dishes : UserControl
    {
        string query = @"SELECT 
                         menu.menu_id, 
                         menu.name AS 'Наименование', 
                         menu.description AS 'Описание', 
                         categories.name AS 'Категория',  
                         CONCAT(menu.price, ' ₽') AS 'Цена',
                         menu.terminalStatus

                         FROM 
                             menu
                         INNER JOIN 
                         categories ON menu.category_id = categories.category_id Where  menu.category_id<7
                        ";
        public Dishes()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
            switch (SafeData.role)
            {
                case "Администратор":
                    AddBtn.Visibility = Visibility.Collapsed;
                    EditBtn.Visibility = Visibility.Collapsed;
                    ShowBtn.Visibility = Visibility.Collapsed;
                    HideBtn.Visibility = Visibility.Collapsed;
                    break;
                case "Менеджер":
                    AddBtn.Visibility = Visibility.Collapsed;
                    EditBtn.Visibility = Visibility.Collapsed;
                    ShowBtn.Visibility = Visibility.Collapsed;
                    HideBtn.Visibility = Visibility.Collapsed;
                    break;
            }
            UpdateDataGridView(query);
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteringAndSorting();
        }

        private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)Sorting.Template.FindName("textBlock", Sorting);
            if (Sorting.SelectedItem == null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            filteringAndSorting();
        }

        private void Filtering_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)Filtering.Template.FindName("textBlock", Filtering);
            if (Filtering.SelectedItem == null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            filteringAndSorting();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

            DishesAdd dishesAdd = new DishesAdd();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;
            dishesAdd.ShowDialog();
            UpdateDataGridView(query);
            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
        }
    
        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                var selectedRow = dataGridView.SelectedItem as DataRowView;
                blockOne.Visibility = Visibility.Collapsed;
                blockTwo.Visibility = Visibility.Visible;
                if (selectedRow != null)
                {
                    ShowBtn.IsEnabled = true;
                    EditBtn.IsEnabled = true;
                    HideBtn.IsEnabled = true;
                    SafeData.menuId = selectedRow[0].ToString();


                    try
                    {
                        using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
                        {
                            con.Open();

                            MySqlCommand cmd = new MySqlCommand($@"SELECT  products.name As 'Наименование' , products.unit_price As 'Цена за единицу', CONCAT(menu_ingredients.quantity, ' кг') As 'Грамовка'  From menu_ingredients
                                                                   Inner Join products
                                                                   On menu_ingredients.product_id = products.product_id where menu_ingredients.menu_id = {SafeData.menuId}", con);

                            MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                            DataTable dtProduct = new DataTable();

                            da.Fill(dtProduct);

                            dataGridView2.ItemsSource = dtProduct.DefaultView;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
            }
        }

        private void filteringAndSorting()
        {
                        query = @"SELECT menu.menu_id, 
                        menu.name AS 'Наименование', 
                        menu.description AS 'Описание', 
                        categories.name AS 'Категория',  
                        CONCAT(menu.price, ' ₽') AS 'Цена',
                        menu.terminalStatus
                        FROM menu 
                        INNER JOIN categories ON menu.category_id = categories.category_id ";

            string sortOrder = null;

            if (Sorting.SelectedItem != null)
            {
                string selectedSortValue = (Sorting.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (selectedSortValue)
                {
                    case "По возврастанию":
                        sortOrder = "ORDER BY menu.name ASC";
                        break;
                    case "По убыванию":
                        sortOrder = "ORDER BY menu.name DESC";
                        break;
                }
            }

            bool hasWhereClause = false;

            if (Filtering.SelectedItem != null)
            {
                string selectedTypeValue = (Filtering.SelectedItem as ComboBoxItem)?.Content.ToString();
                query += $" WHERE categories.name = '{selectedTypeValue}'";
                hasWhereClause = true; 
            }

            string filterText = searchBox.Text;

            if (!string.IsNullOrEmpty(filterText))
            {
                if (hasWhereClause)
                {
                    query += $" AND (menu.name LIKE '%{filterText}%' OR menu.description LIKE '%{filterText}%')";
                }
                else
                {
                    query += $" WHERE (menu.name LIKE '%{filterText}%' OR menu.description LIKE '%{filterText}%')";
                    hasWhereClause = true; 
                }
            }
            query += (hasWhereClause ? " AND" : " WHERE") + " menu.category_id < 7";

            if (sortOrder != null)
            {
                query += " " + sortOrder;
            }

            UpdateDataGridView(query);
        }

        private void UpdateDataGridView(string query)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(MySqlCon.con))
            {
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, connection);
                connection.Open();
                try
                {
                    dataAdapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при извлечении данных: {ex.Message}");
                }
            }
            dataGridView.ItemsSource = dataTable.DefaultView;
            dataGridView.Columns[0].Visibility = Visibility.Hidden;
            dataGridView.Columns[2].Width = 300;
            dataGridView.Columns[4].Width = 200;
            dataGridView.Columns[5].Visibility = Visibility.Hidden;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            query = @"SELECT 
                         menu.menu_id, 
                         menu.name AS 'Наименование', 
                         menu.description AS 'Описание', 
                         categories.name AS 'Категория',  
                         CONCAT(menu.price, ' ₽') AS 'Цена' 
                         FROM 
                             menu
                         INNER JOIN 
                         categories ON menu.category_id = categories.category_id Where  menu.category_id<7
                        ";

            searchBox.Clear();
            Filtering.SelectedItem = null;
            Sorting.SelectedItem = null;
            UpdateDataGridView(query);
            HideBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
            ShowBtn.IsEnabled = false;
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

            DishesEdit dishesEdit = new DishesEdit();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;
            dishesEdit.ShowDialog();
            UpdateDataGridView(query);
            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
        }

        private void hideBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();

                if (MessageBox.Show("Вы уверены, что хотите скрыть это блюдо с терминала?\n\nЭто действие можно будет отменить.", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (MySqlCommand cmd = new MySqlCommand($"Update  menu Set terminalStatus = 'Скрыть' Where menu_id = '{SafeData.menuId}'", con))
                    {

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            UpdateDataGridView(query);
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
        }
        private void ShowBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();

                if (MessageBox.Show("Вы уверены, что хотите показть это блюдо на терминале?\n\nЭто действие можно будет отменить.", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (MySqlCommand cmd = new MySqlCommand($"Update  menu Set terminalStatus = 'Показать' Where menu_id = '{SafeData.menuId}'", con))
                    {

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            UpdateDataGridView(query);
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
            EditBtn.IsEnabled = false;
        }
    }
}
