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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using restarauntV2._0.Forms;
using restarauntV2._0.Forms.Waiter;
using restarauntV2._0.Utilites;

namespace restarauntV2._0.View.Waiter
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as RadioButton;

            SafeData.categoriesId = btn.Uid;

            BlurEffect blurEffect = new BlurEffect
            {
                Radius = 5
            };

            Window1 window = new Window1();

            Blur.workTable.Effect = blurEffect;
            Blur.workTable.IsEnabled = false;
            Blur.workTable.Opacity = 0.5;
            this.Effect = blurEffect;
            this.IsEnabled = false;
            this.Opacity = 0.5;
            window.ShowDialog();

            Blur.workTable.Effect = null;
            Blur.workTable.IsEnabled = true;
            Blur.workTable.Opacity = 1;
            this.Effect = null;
            this.IsEnabled = true;
            this.Opacity = 1;

        }
    }
}
