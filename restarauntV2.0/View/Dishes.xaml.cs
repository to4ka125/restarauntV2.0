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
                         Menu.menu_id, 
                         Menu.name AS 'Наименование', 
                         Menu.description AS 'Описание', 
                         Categories.name AS 'Категория',  
                         CONCAT(Menu.price, ' ₽') AS 'Цена',
                         Menu.terminalStatus

                         FROM 
                             Menu
                         INNER JOIN 
                         Categories ON Menu.category_id = Categories.category_id Where  Menu.category_id<7
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

                            MySqlCommand cmd = new MySqlCommand($@"SELECT  Products.name As 'Наименование' , Products.unit_price As 'Цена за единицу', CONCAT(Menu_Ingredients.quantity, ' кг') As 'Грамовка'  From Menu_Ingredients
                                                                   Inner Join Products
                                                                   On Menu_Ingredients.product_id = Products.product_id where Menu_Ingredients.menu_id = {SafeData.menuId}", con);

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
                        query = @"SELECT Menu.menu_id, 
                        Menu.name AS 'Наименование', 
                        Menu.description AS 'Описание', 
                        Categories.name AS 'Категория',  
                        CONCAT(Menu.price, ' ₽') AS 'Цена',
                        Menu.terminalStatus
                        FROM Menu 
                        INNER JOIN Categories ON Menu.category_id = Categories.category_id ";

            string sortOrder = null;

            if (Sorting.SelectedItem != null)
            {
                string selectedSortValue = (Sorting.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (selectedSortValue)
                {
                    case "По возврастанию":
                        sortOrder = "ORDER BY Menu.name ASC";
                        break;
                    case "По убыванию":
                        sortOrder = "ORDER BY Menu.name DESC";
                        break;
                }
            }

            bool hasWhereClause = false;

            if (Filtering.SelectedItem != null)
            {
                string selectedTypeValue = (Filtering.SelectedItem as ComboBoxItem)?.Content.ToString();
                query += $" WHERE Categories.name = '{selectedTypeValue}'";
                hasWhereClause = true; 
            }

            string filterText = searchBox.Text;

            if (!string.IsNullOrEmpty(filterText))
            {
                if (hasWhereClause)
                {
                    query += $" AND (Menu.name LIKE '%{filterText}%' OR Menu.description LIKE '%{filterText}%')";
                }
                else
                {
                    query += $" WHERE (Menu.name LIKE '%{filterText}%' OR Menu.description LIKE '%{filterText}%')";
                    hasWhereClause = true; 
                }
            }
            query += (hasWhereClause ? " AND" : " WHERE") + " Menu.category_id < 7";

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
                         Menu.menu_id, 
                         Menu.name AS 'Наименование', 
                         Menu.description AS 'Описание', 
                         Categories.name AS 'Категория',  
                         CONCAT(Menu.price, ' ₽') AS 'Цена' 
                         FROM 
                             Menu
                         INNER JOIN 
                         Categories ON Menu.category_id = Categories.category_id Where  Menu.category_id<7
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
                    using (MySqlCommand cmd = new MySqlCommand($"Update  Menu Set terminalStatus = 'Скрыть' Where menu_id = '{SafeData.menuId}'", con))
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
                    using (MySqlCommand cmd = new MySqlCommand($"Update  Menu Set terminalStatus = 'Показать' Where menu_id = '{SafeData.menuId}'", con))
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
