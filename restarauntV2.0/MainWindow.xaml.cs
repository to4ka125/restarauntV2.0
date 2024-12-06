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
using System.Drawing;
using System.Windows.Media.Animation;
using System.Threading;

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
            if (loginBox.Text == Properties.Settings.Default.LocalAdminLogin)
            {
                if (passwordBox.Password == Properties.Settings.Default.LocalAdminPwd)
                {
                    MessageBox.Show("Вы успешно авторизованы!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    workTableLocalAdmin workTableLocalAdmin = new workTableLocalAdmin();
                    this.Visibility = Visibility.Collapsed;
                    workTableLocalAdmin.ShowDialog();
                    this.Visibility = Visibility.Visible;
                    loginBox.Clear();
                    passwordBox.Clear();
                    return;
                }
                else
                {
                    MessageBox.Show("К сожалению, введённый пароль неверен. Пожалуйста, попробуйте снова.",
                                         "Ошибка входа",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);

                    MessageBox.Show("Пройдите капчу");
                    Grid.SetColumn(BorderAnimation, 1);
                    passwordBox.Clear();
                    return;
                }
            }

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
                            MessageBox.Show("Пройдите капчу");
                            Grid.SetColumn(BorderAnimation, 1);
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
            captchaText = GenerateRandomText(6);
            Bitmap captchaBitmap = GenerateCaptchaImage(captchaText);
            Captcha.Source = ConvertBitmapToBitmapSource(captchaBitmap);
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

            private Bitmap GenerateCaptchaImage(string text)
            {
                int width = 250;
                int height = 100; // высота изображения

                // Создаем изображение
                Bitmap bitmap = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    Random rand = new Random();

                    // Заливаем фон градиентом
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int gray = rand.Next(256);
                            g.FillRectangle(new SolidBrush(System.Drawing.Color.FromArgb(gray, gray, gray)), x, y, 1, 1);
                        }
                    }

                    // Настройки шрифта
                    Font font = new Font("Arial", 20, System.Drawing.FontStyle.Bold);
                    System.Drawing.Brush textBrush = System.Drawing.Brushes.Black;

                    // Измеряем ширину текста
                    SizeF textSize = g.MeasureString(text, font);

                    // Вычисляем начальную позицию по X для центрирования
                    float startX = (width - textSize.Width) / 2;

                    // Рисуем текст с волной
                    for (int i = 0; i < text.Length; i++)
                    {
                        float waveHeight = (float)(Math.Sin(i + DateTime.Now.Second) * 10); // высота волны                    
                        g.DrawString(text[i].ToString(), font, textBrush, new PointF(startX + i * 18, height / 2 + waveHeight));
                    }
                }

                return bitmap;
            }
        string captchaText = String.Empty;

        private string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            captchaText = GenerateRandomText(6);
            Bitmap captchaBitmap = GenerateCaptchaImage(captchaText);
            Captcha.Source = ConvertBitmapToBitmapSource(captchaBitmap);
        }

        private BitmapSource ConvertBitmapToBitmapSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap(); 
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(hBitmap);

            return bitmapSource;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (captchaText == CaptchaBox.Text)
            {
                MessageBox.Show("Капча пройдена");
                Grid.SetColumn(BorderAnimation, 0);
            }
            else
            {
                MessageBox.Show("Капча не пройдена");
                Thread.Sleep(30000);
                MessageBox.Show("Форма заблокирован на 30 секунд");
                Grid.SetColumn(BorderAnimation, 0);
            }
        }
    }
}
