using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace restarauntV2._0.View
{
    /// <summary>
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class Order : UserControl
    {
        string query = @"SELECT order_id As 'Номер заказа', concat(us.lastName,' ',Left(us.name,1),'.') AS 'ФИО', o.table_number As 'Номер стола',
                         o.status As 'Статус', o.order_time As 'Дата заказа', Concat (total_price, ' ₽') As 'Стоимость заказа' From orders o
                         inner Join users us On us.user_id  = o.user_id";
        string id;
        public Order()
        {
            InitializeComponent();
        }

        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            blockOne.Visibility = Visibility.Collapsed;
            blockTwo.Visibility = Visibility.Visible;
            if (dataGridView.SelectedItem != null)
            {
                completeBtn.IsEnabled = true;
                cancelBtn.IsEnabled = true;

                var selectedRow = dataGridView.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    id = selectedRow[0].ToString();

                    try
                    {
                        using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand($@"SELECT order_id, m.name As 'Наименование блюда', oi.quantity As 'Количество' FROM restaurant.order_items oi 
                                                                            inner join menu m on m.menu_id = oi.menu_id
                                                                            where order_id = {id}; ", con))
                            {
                                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                da.Fill(dt);

                                dataGridView2.ItemsSource = dt.DefaultView;

                                dataGridView2.Columns[0].Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                if (MessageBox.Show($"Заказ №{id} готов к выдаче?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand($"Update orders Set status = 'Завершен' where order_id ='{id}' ", con))
                    {
                        cmd.ExecuteNonQuery();
                        UpdateDataGridView(query);
                        MessageBox.Show("Данные о заказе обновленны");
                    }
                }
            }
        }
        private void filteringAndSorting()
        {
            query = @"SELECT order_id As 'Номер заказа', us.name As 'Имя официанта', 
                         us.lastName As 'Фамилия официанта', o.table_number As 'Номер стола',
                         o.status As 'Статус', o.order_time As 'Дата заказа', Concat (total_price, ' ₽') As 'Стоимость заказа' From orders o
                         inner Join users us On us.user_id  = o.user_id";

            bool hasWhereClause = false;

            if (Filtering.SelectedItem != null)
            {
                string selectedStatusValue = (Filtering.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (!string.IsNullOrEmpty(selectedStatusValue))
                {
                    if (!hasWhereClause)
                    {
                        query += " WHERE";
                        hasWhereClause = true;
                    }
                    else
                    {
                        query += " AND";
                    }
                    query += $" o.status = '{selectedStatusValue}'";
                }
            }

            if (!string.IsNullOrEmpty(datePicker.Text))
            {
                string selectedTime = datePicker.SelectedDate?.ToString("yyyy-MM-dd") ?? string.Empty;
                if (!hasWhereClause)
                {
                    query += " WHERE";
                    hasWhereClause = true;
                }
                else
                {
                    query += " AND";
                }
                query += $" (o.order_time  LIKE '%{selectedTime}%')";
            }

            UpdateDataGridView(query);
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
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            completeBtn.IsEnabled = false;
            cancelBtn.IsEnabled = false;

            switch (SafeData.role)
            {
                case "Администратор":
                    completeBtn.Visibility = Visibility.Collapsed;

                    break;
                case "Менеджер":
                    completeBtn.Visibility = Visibility.Collapsed;
                    break;
                case "Шеф":
                    cancelBtn.Visibility = Visibility.Collapsed;
                    break;
                case "Официант":
                    cancelBtn.Visibility = Visibility.Collapsed;
                    completeBtn.Visibility = Visibility.Collapsed;
                    break;
            }
            UpdateDataGridView(query);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                if (MessageBox.Show($"Вы уверенны что хотите отменить заказ №{id}?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand($"Update orders Set status = 'Отменен' where order_id ='{id}' ", con))
                    {
                        cmd.ExecuteNonQuery();
                        UpdateDataGridView(query);
                        MessageBox.Show("Данные о заказе обновленны");
                    }
                }

            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            query = @"SELECT order_id As 'Номер заказа', us.name As 'Имя официанта', 
                         us.lastName As 'Фамилия официанта', o.table_number As 'Номер стола',
                         o.status As 'Статус', o.order_time As 'Дата заказа', Concat (total_price, ' ₽') As 'Стоимость заказа' From orders o
                         inner Join users us On us.user_id  = o.user_id";
            datePicker.SelectedDate = null;
            Filtering.SelectedItem = null;
            UpdateDataGridView(query);
            MessageBox.Show("Фильтры успешно очищены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            filteringAndSorting();
        }
    }
}
