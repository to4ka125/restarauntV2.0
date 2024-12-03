using Microsoft.Win32;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using restarauntV2._0.Forms;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace restarauntV2._0.View
{
    /// <summary>
    /// Interaction logic for Products.xaml
    /// </summary>
    public partial class Products : UserControl
    {
        string query = @"SELECT 
                             Products.product_id, 
                             Products.name AS 'Наименование', 
                             Concat( Products.quantity,' кг.') AS 'Остаток на складе', 
                             CONCAT( Products.unit_price, ' руб.') AS 'Цена за кг',
                             Supplier.name AS 'Поставщик'                                                       
                             FROM 
                               Products
                               INNER JOIN 
                               Supplier ON Products.supplier_id = Supplier.supplier_id";
        public Products()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            switch (SafeData.role)
            {
                case "Шеф":
                    AddBtn.Visibility = Visibility.Collapsed;
                    EditBtn.Visibility = Visibility.Collapsed;
                    ReportBtn.Visibility = Visibility.Collapsed;
                    break;
                case "Администратор":
                    AddBtn.Visibility = Visibility.Collapsed;
                    EditBtn.Visibility = Visibility.Collapsed;
                    break;
            }
            EditBtn.IsEnabled = false;
            UpdateDataGridView(query);
        }

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            filteringAndSorting();
        }

        private void Sorting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            TextBlock textBlock = (TextBlock)Sorting.Template.FindName("textBlock", Sorting);
            textBlock.Visibility = Visibility.Collapsed;
            filteringAndSorting();
        }

        private void filteringAndSorting()
        {
            query = @"SELECT 
                             Products.product_id, 
                             Products.name AS 'Наименование', 
                             Concat( Products.quantity,' кг.') AS 'Остаток на складе', 
                             CONCAT( Products.unit_price, ' руб.') AS 'Цена за кг',
                             Supplier.name AS 'Поставщик'                                                       
                             FROM 
                               Products
                               INNER JOIN 
                               Supplier ON Products.supplier_id = Supplier.supplier_id";

            string sortOrder = null;
            if (Sorting.SelectedItem != null)
            {
                string selectedSortValue = (Sorting.SelectedItem as ComboBoxItem)?.Content.ToString();
                switch (selectedSortValue)
                {
                    case "По возврастанию":
                        sortOrder = "ORDER BY Products.name ASC";
                        break;
                    case "По убыванию":
                        sortOrder = "ORDER BY Products.name DESC";
                        break;
                }
            }

            string filterText = searchBox.Text;
            if (!string.IsNullOrEmpty(filterText))
            {
                    query += $" WHERE (Products.name LIKE '%{filterText}%')";
            }

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
            dataGridView.Columns[2].Width = 300;
            dataGridView.Columns[4].Width = 200;
            dataGridView.Columns[0].Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

            ProductsAdd productsAdd = new ProductsAdd();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;

            productsAdd.ShowDialog();
            UpdateDataGridView(query);
            EditBtn.IsEnabled = false;
            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;
        }

        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                EditBtn.IsEnabled = true;
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    SafeData.product_id = selectedRow[0].ToString();
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            query = @"SELECT 
                             Products.product_id, 
                             Products.name AS 'Наименование', 
                             Concat( Products.quantity,' кг.') AS 'Остаток на складе', 
                             CONCAT( Products.unit_price, ' руб.') AS 'Цена за кг',
                             Supplier.name AS 'Поставщик'                                                       
                             FROM 
                               Products
                               INNER JOIN 
                               Supplier ON Products.supplier_id = Supplier.supplier_id";
            Sorting.SelectedItem = null;
            UpdateDataGridView(query);
            MessageBox.Show("Фильтры успешно очищены.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

            ProductsEdit productsEdit = new ProductsEdit();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;

            productsEdit.ShowDialog();
            EditBtn.IsEnabled = false;
            UpdateDataGridView(query);
            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;
        }

        private void ReportBtn_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string query = @"SELECT   Products.product_id As 'Номер продукта', 
                             Products.name AS 'Наименование', 
                             Concat(Products.quantity, ' кг.') AS 'Остаток на складе', 
                             CONCAT(Products.unit_price, ' руб.') AS 'Цена за кг',
                             Supplier.name AS 'Поставщик'
                             FROM
                               Products
                               INNER JOIN
                               Supplier ON Products.supplier_id = Supplier.supplier_id WHERE Products.quantity < 5";

            using (MySqlConnection connection = new MySqlConnection(MySqlCon.con))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                    return;
                }
                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Отчет");
                    string reportTitle = $"Отчет от {DateTime.Now.ToString("dd.MM.yyyy")}, продукты необходимые для закупки";
                    worksheet.Cells[1, 1].Value = reportTitle;
                    worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Merge = true;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells[1, 1].Style.Font.Size = 16;
                    worksheet.Cells[1, 1].Style.Font.Bold = true;

                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        worksheet.Cells[2, i + 1].Value = dataTable.Columns[i].ColumnName;
                        worksheet.Cells[2, i + 1].Style.Font.Size = 14;
                    }

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataTable.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 3, j + 1].Value = dataTable.Rows[i][j];
                            worksheet.Cells[i + 3, j + 1].Style.Font.Size = 12;
                        }
                    }
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                    saveFileDialog.Title = "Сохранить отчет";
                    saveFileDialog.FileName = $"Отчет_{DateTime.Now:dd-MM-yyyy}.xlsx";
                   
                    
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string filePath = saveFileDialog.FileName;
                        FileInfo fileInfo = new FileInfo(filePath);
                        excelPackage.SaveAs(fileInfo);
                        MessageBox.Show("Отчет сохранен: " + filePath);
                    }
                }
            }
        }
    }
}
