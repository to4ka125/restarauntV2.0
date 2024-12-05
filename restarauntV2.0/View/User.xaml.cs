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
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class User : UserControl
    {

        public User()
        {
            InitializeComponent();
        }

        private void UpdateDataGrid()
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                try
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"Select user_id, name As 'Имя', lastName 'Фамилия', login As 'Логин', role As 'Роль', 
                                                           email As 'Почта', phone As 'Телефон' From users ", con);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);


                    foreach (DataRow row in dt.Rows)
                    {
                        string email = row["Почта"].ToString();
                        string phone = row["Телефон"].ToString();
                        string login = row["Логин"].ToString();
                        int visibleDigits = 16; 

                        if (phone.Length > visibleDigits)
                        {   
                            row["Телефон"] = phone.Substring(0, visibleDigits) + new string('*', phone.Length - visibleDigits);
                        }
                        else
                        {
                            row["Телефон"] = new string('*', phone.Length);
                        }
                        if (email.Length > 4)
                        {
                            row["Почта"] = email.Substring(0, 4) + new string('*', email.Length - 4);
                        }
                        else
                        {
                            row["Почта"] = new string('*', email.Length);
                        }

                        if (login.Length>4)
                        {
                            row["Логин"] = login.Substring(0, 4) + new string('*',login.Length-4); 
                        }
                        else
                        {
                            row["Логин"] = new string('*', login.Length);
                        }


                    }
                    dataGridView.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }

                dataGridView.Columns[0].Visibility = Visibility.Collapsed;
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            EditBtn.IsEnabled = false;
            DelBtn.IsEnabled = false;
            UpdateDataGrid();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

           UserAdd UserAdd = new UserAdd();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;

            UserAdd.ShowDialog();
            UpdateDataGrid();
            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

            UserEdit UserEdit = new UserEdit();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;

            UserEdit.ShowDialog();
            UpdateDataGrid();
            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();

                if (MessageBox.Show("Вы хотите удалить выбранного пользователя?","Предупреждение",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
                {
                    using (MySqlCommand cmd = new MySqlCommand($@"Delete From Users where user_id= '{SafeData.userIdEdit}'",con))
                    {
                        MessageBox.Show("Пользователь удален");
                        cmd.ExecuteNonQuery();
                        UpdateDataGrid();
                    }
                }
          
            }
        }

        private void dataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridView.SelectedItem != null)
            {
                EditBtn.IsEnabled = true;
                DelBtn.IsEnabled = true;
                var selectedRow = dataGridView.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    SafeData.userIdEdit = selectedRow[0].ToString();
                }
            }
        }
    }
}
