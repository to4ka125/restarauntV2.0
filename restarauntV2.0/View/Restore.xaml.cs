using MySql.Data.MySqlClient;
using restarauntV2._0.Utilites;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace restarauntV2._0.View
{
    /// <summary>
    /// Interaction logic for Restore.xaml
    /// </summary>
    public partial class Restore : UserControl
    {
        public Restore()
        {
            InitializeComponent();
        }

        private void RestoreBtn_Click(object sender, RoutedEventArgs e)
        {
            string backupPath = "Backup\\restaraunt.sql"; // Дамп структуры базы данных
            string databaseName = "restaurant"; // База данных

            string conString = "host=127.0.0.1; uid=root;pwd=;";
            using (MySqlConnection con = new MySqlConnection(conString))
            {
                con.Open();
                MySqlCommand cmdCheckExists = new MySqlCommand($"SELECT COUNT(*) FROM information_schema.schemata WHERE schema_name = '{databaseName}';", con);
                int dbExists = Convert.ToInt32(cmdCheckExists.ExecuteScalar());

                if (dbExists > 0)
                {
                    MySqlCommand cmdDrop = new MySqlCommand($"DROP DATABASE IF EXISTS `{databaseName}`;", con);
                    cmdDrop.ExecuteNonQuery();
                }
                MySqlCommand cmdCreate = new MySqlCommand($"CREATE DATABASE `{databaseName}`;", con);
                cmdCreate.ExecuteNonQuery();

                MySqlCommand cmdUse = new MySqlCommand($"USE `{databaseName}`;", con);
                cmdUse.ExecuteNonQuery();

                string script = File.ReadAllText(backupPath);
                MySqlScript sqlScript = new MySqlScript(con, script);
                sqlScript.Execute();

                con.Close();
            }
            MessageBox.Show("Востановление структуры прошло успешно");
        }
    }
}
