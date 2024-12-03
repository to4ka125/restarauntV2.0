using restarauntV2._0.View;
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

namespace restarauntV2._0.Forms
{
    /// <summary>
    /// Interaction logic for workTableLocalAdmin.xaml
    /// </summary>
    public partial class workTableLocalAdmin : Window
    {
        public workTableLocalAdmin()
        {
            InitializeComponent();
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;

            radioButton.IsChecked = true;
            container.Children.Clear();
            switch (radioButton.Name)
            {
                case "Restore":
                    Restore restore = new Restore();
                    container.Children.Add(restore);
                    break;
                   
                case "Import":
                    Import import = new Import();
                    container.Children.Add(import);
                    break;
            }
        }
    }
}
