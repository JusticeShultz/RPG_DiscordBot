using System;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;

namespace RPGBotLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            System.IO.StreamReader file = new System.IO.StreamReader(@"Resources\BotKey.txt");

            KeyEntry.Text = file.ReadToEnd();

            file.Close();
            //KeyEntry
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Start bot
            Process.Start(@"Resources\RPG_Bot.exe");

            //Check our checkbox setting
            if (LauncherCheckbox.IsChecked ?? false)
                Close();
            else WindowState = WindowState.Minimized;
        }

        private void SetKey(object sender, RoutedEventArgs e)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"RPG");
            file.Write(KeyEntry.Text);

            file.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}