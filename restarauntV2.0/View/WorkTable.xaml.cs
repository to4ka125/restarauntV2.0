using System;
using System.Collections.Generic;
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

namespace restarauntV2._0.View
{
    /// <summary>
    /// Interaction logic for WorkTable.xaml
    /// </summary>
    public partial class WorkTable : Window
    {
        public WorkTable()
        {
            InitializeComponent();
        }

        private void menu_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;

            radioButton.IsChecked = true;

            container.Children.Clear();


            switch (radioButton.Name)
            {

                case "Dishes":
                    Dishes dishes = new Dishes();
                    container.Children.Add(dishes);
                    break;

                case "User":
                    User user = new User();
                    container.Children.Add(user);
                    break;
                case "AlcoholicDrinks":
                    AlcoholicDrinks alcoholicDrinks = new AlcoholicDrinks();
                    container.Children.Add(alcoholicDrinks);
                    break;
                case "Drinks":
                    Drinks drinks = new Drinks();
                    container.Children.Add(drinks);
                    break;

                case "Products":
                    Products products = new Products();
                    container.Children.Add(products);
                    break;

                case "Orders":
                    Order order = new Order();
                    container.Children.Add(order);
                    break;

                case "Menu":
                    Waiter.Menu menu = new Waiter.Menu();
                    container.Children.Add(menu);
                    break;
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Role.Text = Utilites.SafeData.role;
            UserName.Text = Utilites.SafeData.userName;
            switch (Utilites.SafeData.role) {
                case "Администратор":
                    imageRole.Source = new BitmapImage(new Uri("/Images/IconUser/Admin.png", UriKind.Relative));
                    Menu.Visibility = Visibility.Collapsed;
                    break;
                case "Менеджер":
                    imageRole.Source = new BitmapImage(new Uri("/Images/IconUser/Manager.png", UriKind.Relative));
                    Menu.Visibility = Visibility.Collapsed;
                    User.Visibility = Visibility.Collapsed;
                    break;
                case "Шеф":
                    imageRole.Source = new BitmapImage(new Uri("/Images/IconUser/Chef.png", UriKind.Relative));
                    Menu.Visibility = Visibility.Collapsed;
                    User.Visibility = Visibility.Collapsed;
                    Report.Visibility = Visibility.Collapsed;
                    break;
                case "Официант":
                    Products.Visibility = Visibility.Collapsed;
                    User.Visibility = Visibility.Collapsed;
                    AlcoholicDrinks.Visibility = Visibility.Collapsed;
                    Drinks.Visibility = Visibility.Collapsed;
                    Dishes.Visibility = Visibility.Collapsed;
                    Report.Visibility = Visibility.Collapsed;
                    imageRole.Source = new BitmapImage(new Uri("/Images/IconUser/Waiter.png", UriKind.Relative));
                    break;
                default:

                    break;
            }

        }
    }
}
