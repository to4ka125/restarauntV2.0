using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using Word = Microsoft.Office.Interop.Word;
using restarauntV2._0.Model;
using restarauntV2._0.Utilites;

namespace restarauntV2._0.Forms.Waiter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();

            Basket.Instance.PropertyChanged += BasketInstance_PropertyChanged;
        }

        private void BasketInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateDataTable();
        }

        DataTable dataTable;
        private readonly string FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "check.docx");

        private void UpdateDataTable()
        {
            if (Basket.basket.Count > 0)
            {
                AddOrderBtn.IsEnabled = true;
            } 
            else
            {
                AddOrderBtn.IsEnabled = false;
            }

            dataTable = dataTable = new DataTable();
            decimal totalPrice=0;

            dataTable.Columns.Add("countOrder");
            dataTable.Columns.Add("name");
            dataTable.Columns.Add("quantity");
            dataTable.Columns.Add("price");

            int countOrder = 0;
            foreach (var item in Basket.basket)
            {
                countOrder++;
                string itemId = item.Key;

                int quantity = item.Value;

                using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand($"Select name,price From Menu Where menu_id ='{itemId}'", con);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                 
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string name = reader["name"].ToString();
                            decimal price = Convert.ToDecimal(reader["price"]);
                            dataTable.Rows.Add(countOrder, name, quantity, price);
                        }
                    }
                }
            }
            foreach (DataRow row in dataTable.Rows )
            {
                decimal price = Convert.ToDecimal(row["price"]);

                int quantity = Convert.ToInt32(row["quantity"]);

               totalPrice += price * quantity;
                
            }
            TotalPrice.Text = totalPrice.ToString();
            MenuDataGrid.ItemsSource = dataTable.DefaultView;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            switch (SafeData.categoriesId)
            {
                case "1": Title.Text = "ЗАКУСКИ";
                    break;

                case "2":
                    Title.Text = "САЛАТЫ";
                    break;

                case "3":
                    Title.Text = "СУПЫ";
                    break;

                case "4":
                    Title.Text = "СУШИ И САШИМИ";
                    break;

                case "5":
                    Title.Text = "ОСНОВНЫЕ БЛЮДА";
                    break;

                case "6":
                    Title.Text = "ДЕСЕРТЫ";
                    break;

                case "7":
                    Title.Text = "БЕЗАЛКОГОЛЬНЫЕ НАПИТКИ";
                    break;

                case "8":
                    Title.Text = "АЛКОГОЛЬНЫЕ НАПИТКИ";
                    break;
            }
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();

                MySqlCommand cmd = new MySqlCommand($"Select menu_id,name,price,Image From Menu where category_id = '{SafeData.categoriesId}' And terminalStatus = 'Показать'", con);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                ImageSource image;

                for (int i = 0; i < dt.Rows.Count; i += 3)
                {
                    StackPanel stackPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 10, 0, 10)
                    };

                    container.Children.Add(stackPanel);
                    for (int j = 0; j < 3; j++)
                    {
                        if (i + j < dt.Rows.Count)
                        {
                            image = new BitmapImage(new Uri($"/Images/ImagesMenu/{dt.Rows[i + j]["Image"]}", UriKind.RelativeOrAbsolute));
                            var menuId = dt.Rows[i + j]["menu_id"];
                            var Categories = new Categories()
                            {
                                Uid = menuId.ToString(),
                                Source = image,
                                Title = dt.Rows[i + j]["name"].ToString(),
                                Margin = new Thickness(0, 0, 20, 0)
                            };
                            stackPanel.Children.Add(Categories);
                        }
                    }
                }
            }
            UpdateDataTable();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int maxOrder_id;
            if (TableBox.Text == "")
            {
                MessageBox.Show("Пожалуйста, выберите столик для вашего заказа.",
                 "Выбор столика",
                 MessageBoxButton.OK,
                 MessageBoxImage.Question);
                return;
            }

            string[] table = TableBox.Text.Split(' ');
            string tableId = table[1];
            using (MySqlConnection con = new MySqlConnection(Utilites.MySqlCon.con))
            {
                con.Open();


                using (MySqlCommand cmd = new MySqlCommand($@"Insert into Orders (user_id,table_number,status,order_time,total_price)
                                                              Values ('{SafeData.userId}','{tableId}','В обработке','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}','{TotalPrice.Text.Replace(',','.')}') ",con))
                {
                    cmd.ExecuteNonQuery();
                }




                using (MySqlCommand cmd = new MySqlCommand("SELECT MAX(order_id) FROM Orders", con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    maxOrder_id = int.Parse(dt.Rows[0].ItemArray[0].ToString());
                }


                foreach (var item in Basket.basket)
                {
                    string menuId = item.Key;
                    int quantity = item.Value;
                    using (MySqlCommand cmd = new MySqlCommand($@"INSERT INTO `restaurant`.`Order_Items` (`order_id`, `menu_id`, `quantity`)
                                                                  Values ('{maxOrder_id}','{menuId}','{quantity}')",con))
                    {
                     cmd.ExecuteNonQuery();
                    }


                    for (int i = 0; i < quantity; i++)
                    {
                        using (MySqlCommand cmd = new MySqlCommand($@"
                        UPDATE Products p
                        JOIN Menu_Ingredients mn ON p.product_id = mn.product_id
                        SET p.quantity = p.quantity - mn.quantity
                        WHERE mn.menu_id = '{menuId}'", con))
                        {
                         cmd.ExecuteNonQuery();
                        }
                    }
                }


                MessageBox.Show("Ваш заказ успешно сформирован!",
                "Успех",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


                if (MessageBox.Show("Распечать чек", "Чек", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    var itemsList = new List<string>();
                    var wordApp = new Microsoft.Office.Interop.Word.Application();
                    wordApp.Visible = false;
                    Microsoft.Office.Interop.Word.Document wordDocument;
                    try
                    {
                        wordDocument = wordApp.Documents.Open(FileName);
                    }
                    catch (System.Runtime.InteropServices.COMException)
                    {
                        string localCopyPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "check_copy.docx");
                        File.Copy(FileName, localCopyPath, true);
                        wordDocument = wordApp.Documents.Open(localCopyPath);
                    }
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        string name = dataTable.Rows[i].ItemArray[1].ToString();
                        string quantity = dataTable.Rows[i].ItemArray[2].ToString();
                        string price = dataTable.Rows[i].ItemArray[3].ToString();

                        Word.Table tables = wordDocument.Tables[1];
                        Word.Row newRow = tables.Rows.Add();

                        newRow.Cells[1].Range.Text = name;
                        newRow.Cells[2].Range.Text = quantity;
                        newRow.Cells[3].Range.Text = price;
                    }
                    ReplaceWordStub("{dataTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), wordDocument);
                    ReplaceWordStub("{totalPrice}", TotalPrice.Text, wordDocument);
                }
                TableBox.SelectedItem=null;
                Basket.basket.Clear();
                UpdateDataTable();
                InitializeComponent();
            }
        }
        private void ReplaceWordStub(string stubToReplace, string text, Microsoft.Office.Interop.Word.Document wordDocument)
        {
            var range = wordDocument.Content;
            range.Find.ClearFormatting();
            range.Find.Execute(FindText: stubToReplace, ReplaceWith: text);
        }
        private void TableBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)TableBox.Template.FindName("textBlock", TableBox);
            if (TableBox.SelectedItem == null)
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

           
