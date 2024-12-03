using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for UserAdd.xaml
    /// </summary>
    public partial class UserAdd : Window
    {
        public UserAdd()
        {
            InitializeComponent();
        }

        private void SafeUser_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text.Length == 0 || LastName.Text.Length == 0
                || Login.Text.Length == 0 || Password.Password.Length == 0 || Role.SelectedItem == null)

            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string login = Login.Text;
            if (UserExists(login))
            {
                MessageBox.Show("Пользователь с таким логином уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                try
                {
                    con.Open();

                    string name = Name.Text;
                    string lastName = LastName.Text;
         
                    string password = GetHashPass(Password.Password);
                    string role = Role.Text;
                    string email = Email.Text;
                    string phone = Phone.Text;

                    MySqlCommand cmd = new MySqlCommand($@"Insert into restaurant.Users (name, lastName, login, password, role, email, phone)
                                                          Values ('{name}','{lastName}', '{login}','{password}','{role}','{email}','{phone}')",con);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Пользователь добавлен","Информация",MessageBoxButton.OK,MessageBoxImage.Information);


                    Name.Clear();
                    LastName.Clear();
                    Login.Clear();
                    Password.Clear();
                    Role.Text="";
                    Email.Clear();
                    Phone.Clear();

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка-{ex.Message}"); ;
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBlock textBlock = (TextBlock)Role.Template.FindName("textBlock", Role);

            if (Role.SelectedItem==null)
            {
                textBlock.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
       
        }

        private bool UserExists(string login)
        {
            MySqlConnection con = new MySqlConnection(MySqlCon.con);
            con.Open();
            MySqlCommand cmd = new MySqlCommand($@"SELECT COUNT(*) FROM Users WHERE login = '{login}'", con);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            con.Close();

            return count > 0;
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

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Click_1(object sender, RoutedEventArgs e)
        {
            string Passwd = CreatePassword(10);
            MessageBox.Show($"Сгенерированный пароль: {Passwd}",
                  "Пароль сгенерирован",
                  MessageBoxButton.OK,
                  MessageBoxImage.Information);
            Password.Password = Passwd;
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
            if (Regex.IsMatch(e.Text, @"^[_]$")) { e.Handled = false; }
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
