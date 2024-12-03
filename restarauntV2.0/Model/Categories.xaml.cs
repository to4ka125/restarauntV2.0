using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace restarauntV2._0.Model
{
    /// <summary>
    /// Interaction logic for Categories.xaml
    /// </summary>
    public partial class Categories : UserControl
    {

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Categories));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Categories));
        public Categories()
        {
            InitializeComponent();
        }

        private void DellIngredients_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var parent = FindParent<UserControl>(button);
            string id = parent.Uid;



            int count = int.Parse(CountIngredients.Text);
            if (count != 0)
            {
                CountIngredients.Text = (count - 1).ToString();
                Basket.Instance.DellIngredients(id);
            }
        }
        public static int GetProductCount(string productName)
        {
            return Basket.basket.ContainsKey(productName) ? Basket.basket[productName] : 0;
        }

        private void AddIngredients_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var parent = FindParent<UserControl>(button);
            string id = parent.Uid;

            Basket.Instance.AddToBasket(id);
            using (MySqlConnection con = new MySqlConnection(MySqlCon.con))
            {
                con.Open();
                List<double> availableQuantities = new List<double>();
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT quantity FROM Products", con))
                {
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            availableQuantities.Add(dr.GetDouble(0));
                        }
                    }
                }
                foreach (var item in Basket.basket)
                {
                    string menuId = item.Key;
                    int quantityInBasket = item.Value;

                    List<double> quantitiesInBasket = new List<double>();
                    List<int> product_id = new List<int>();

                    using (MySqlCommand cmd = new MySqlCommand($@"SELECT quantity,product_id FROM Menu_Ingredients WHERE menu_id='{menuId}'", con))

                    {
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                           {
                                product_id.Add(dr.GetInt32(1));
                                quantitiesInBasket.Add(dr.GetDouble(0) * quantityInBasket);
                            }
                        }
                    }
                    for (int i = 0; i < product_id.Count; i++)
                    {
                            availableQuantities[product_id[i]-1] -= quantitiesInBasket[i];
                        if (availableQuantities[product_id[i]-1] <= 0)
                        {
                            Basket.Instance.DellIngredients(id);
                            MessageBox.Show("На складе не достаточно продуктов для приготовленя блюда.","Информация",MessageBoxButton.OK,MessageBoxImage.Information);
                            return;
                        }
                    }
                }
            }
            CountIngredients.Text = GetProductCount(id).ToString();
        }
       

        private T FindParent<T>(DependencyObject child) where T :DependencyObject{
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            return parent ?? FindParent<T>(parentObject);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
      
        }
    }
}
