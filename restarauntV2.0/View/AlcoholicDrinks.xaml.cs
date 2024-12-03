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
    /// Interaction logic for AlcoholicDrinks.xaml
    /// </summary>
    public partial class AlcoholicDrinks : UserControl
    {
        public AlcoholicDrinks()
        {
            InitializeComponent();
        }
        string query = @"SELECT 
                         Menu.menu_id, 
                         Menu.name AS 'Наименование', 
                         Menu.description AS 'Описание', 
                         Categories.name AS 'Категория',  
                         CONCAT(Menu.price, ' ₽') AS 'Цена',
                        terminalStatus
                         FROM 
                             Menu
                         INNER JOIN 
                         Categories ON Menu.category_id = Categories.category_id Where  Menu.category_id=8
                        ";
        private void DataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                ShowBtn.IsEnabled = true;
                HideBtn.IsEnabled = true;
                EditBtn.IsEnabled = true;
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    SafeData.drinks_id = selectedRow[0].ToString();
                }
            }
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
        }

        private void filteringAndSorting()
        {
            query = @"SELECT Menu.menu_id, 
                        Menu.name AS 'Наименование', 
                        Menu.description AS 'Описание', 
                        Categories.name AS 'Категория',  
                        CONCAT(Menu.price, ' ₽') AS 'Цена',
                        terminalStatus
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
            query += (hasWhereClause ? " AND" : " WHERE") + " Menu.category_id = 8";

            if (sortOrder != null)
            {
                query += " " + sortOrder;
            }

            UpdateDataGridView(query);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EditBtn.IsEnabled = false;
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
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

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteringAndSorting();
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
                         Categories ON Menu.category_id = Categories.category_id Where  Menu.category_id=8
                        ";

            searchBox.Clear();
            Sorting.SelectedItem = null;
            UpdateDataGridView(query);
            EditBtn.IsEnabled = false;
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
        }
        private void EdtiBtn_Click(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

            DrinksEdit drinksEdit = new DrinksEdit();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;
            drinksEdit.ShowDialog();
            UpdateDataGridView(query);
            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;
            EditBtn.IsEnabled = false;
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

            DrinksAdd drinksAdd = new DrinksAdd();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;
            drinksAdd.ShowDialog();
            UpdateDataGridView(query);
            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;
            EditBtn.IsEnabled = false;
            ShowBtn.IsEnabled = false;
            HideBtn.IsEnabled = false;
        }

        private void HideBtn_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();

                if (MessageBox.Show("Вы уверены, что хотите показть это блюдо на терминале?\n\nЭто действие можно будет отменить.", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (MySqlCommand cmd = new MySqlCommand($"Update  Menu Set terminalStatus = 'Показать' Where menu_id = '{SafeData.drinks_id}'", con))
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

                if (MessageBox.Show("Вы уверены, что хотите скрыть это блюдо с терминала?\n\nЭто действие можно будет отменить.", "Подтверждение действия", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (MySqlCommand cmd = new MySqlCommand($"Update  Menu Set terminalStatus = 'Скрыть' Where menu_id = '{SafeData.drinks_id}'", con))
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
