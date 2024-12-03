using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using restarauntV2._0.View;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using restarauntV2._0.Forms;
using restarauntV2._0.Forms.Waiter;
using System.Security.Cryptography;

namespace restarauntV2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
        private void passwordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordCheckPsaholder(passwordBox);
        }
        private void passwordCheckPsaholder(PasswordBox passwordBox)
        {
            TextBlock textBlock = (TextBlock)passwordBox.Template.FindName("textBlock", passwordBox);
            if (passwordBox.Password.Length > 0)
            {
                textBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlock.Visibility = Visibility.Visible;
            }
        }
        private void loginBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (Regex.IsMatch(e.Text, @"^[\W]$")) { e.Handled = true; }
            if (Regex.IsMatch(e.Text, @"^[а-яА-Я]$")) { e.Handled = true; }
        }

        private void signIN_Click(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                try
                {
                    con.Open();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка с подключением", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string login = loginBox.Text;
                string password = passwordBox.Password;


                MySqlCommand cmd = new MySqlCommand($"Select * From Users where login = '{login}'", con);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);

                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string passwordBd = dt.Rows[0].ItemArray[4].ToString();

                    if (passwordBd == GetHashPass(password))
                    {
                        SafeData.role = dt.Rows[0].ItemArray[5].ToString();
                        SafeData.userName = $"{dt.Rows[0].ItemArray[1]} {dt.Rows[0].ItemArray[2]}";
                        SafeData.userId = dt.Rows[0].ItemArray[0].ToString();
                        MessageBox.Show("Вы успешно авторизованы!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                        Blur.workTable = new WorkTable();
                        this.Visibility = Visibility.Collapsed;
                        Blur.workTable.ShowDialog();
                        this.Visibility = Visibility.Visible;
                        passwordBox.Clear();
                    }
                    else
                    {
                        if (passwordBd != GetHashPass(password))
                        {
                            MessageBox.Show("К сожалению, введённый пароль неверен. Пожалуйста, попробуйте снова.",
                                            "Ошибка входа",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                            passwordBox.Clear();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Пользователь не найден.\nПожалуйста, проверьте введенные данные и попробуйте снова.",
                     "Ошибка",
                     MessageBoxButton.OK,
                     MessageBoxImage.Warning);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Вы действительно хотите выйти из приложения?",
                       "Подтверждение выхода",
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
