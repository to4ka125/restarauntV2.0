using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for UserEdit.xaml
    /// </summary>
    public partial class UserEdit : Window
    {
        public UserEdit()
        {
            InitializeComponent();
        }

        private void SafeUser_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text == null || LastName.Text == null
             || Role.SelectedItem == null || Login.Text==null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();
                string query;

                if (Password.Password == "")
                {
                    query = $@"Update 
                               users set name ='{Name.Text}',
                               lastName = '{LastName.Text}',
                               login = '{Login.Text}',
                               role = '{Role.Text}', 
                               email ='{Email.Text}',
                               phone = '{Phone.Text}'
                               where user_id='{SafeData.userIdEdit}'"; 
                }
                else
                {
                    query = $@"Update 
                               users set name ='{Name.Text}',
                               lastName = '{LastName.Text}', 
                               login = '{Login.Text}',
                               password ='{GetHashPass(Password.Password)}',
                               role = '{Role.Text}', 
                               email = '{Email.Text}', 
                               phone = '{Phone.Text}' 
                               where user_id='{SafeData.userIdEdit}'";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                

                    if (MessageBox.Show("Вы хотите изменить пользователя?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        MessageBox.Show($"Пользователь под логином: {Login.Text} был успешно изменен.", "Успешное изменение",
                                           MessageBoxButton.OK,
                                             MessageBoxImage.Information);
                        cmd.ExecuteNonQuery();
                        this.Close();
                    }
           
               
                }
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            string Passwd = CreatePassword(10);
            MessageBox.Show($"Сгенерированный пароль: {Passwd}",
               "Пароль сгенерирован",
               MessageBoxButton.OK,
               MessageBoxImage.Information);
            Password.Password = Passwd;
        }
        public static string GetHashPass(string password)
        {

            byte[] bytesPass = Encoding.UTF8.GetBytes(password);

            SHA256Managed hashstring = new SHA256Managed();

            byte[] hash = hashstring.ComputeHash(bytesPass);

            string hashPasswd = string.Empty;

            foreach (byte x in hash)
            {
                hashPasswd += String.Format("{0:x2}", x);
            }

            hashstring.Dispose();

            return hashPasswd;
        }
        private void Role_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)Role.Template.FindName("textBlock",Role);

            if (Role.SelectedItem==null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
        }

        private void Btn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection  con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand($"Select * From users Where user_id = '{SafeData.userIdEdit}'",con))
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    Name.Text = dt.Rows[0].ItemArray[1].ToString();
                    LastName.Text = dt.Rows[0].ItemArray[2].ToString();
                    Login.Text = dt.Rows[0].ItemArray[3].ToString();
                    Role.Text = dt.Rows[0].ItemArray[5].ToString();
                    Phone.Text = dt.Rows[0].ItemArray[7].ToString();
                    Email.Text = dt.Rows[0].ItemArray[6].ToString();
                }
            }
        }

        private void Name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[A-Za-z]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void LastName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[0-9\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[A-Za-z]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void Phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-ЯA-Za-z \W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = true; }
        }

        private void Email_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-Я \W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_.]$")) { e.Handled = false; }
            if (Regex.IsMatch(e.Text, @"^[@]$")) { e.Handled = false; }
        }

        private void Login_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-Я \W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = false; }
        }

        private void Password_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[а-яА-Я \W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = false; }
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)Password.Template.FindName("textBlock", Password);
            if (Password.Password.Length > 0)
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
