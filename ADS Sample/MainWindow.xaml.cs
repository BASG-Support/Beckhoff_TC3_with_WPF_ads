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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADS_Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ADS_Sample.UI_Config.ui_manager.Load_DiagConfig();
        }

        private void navi_diagnostic_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new Diagnostics());
        }

        private void navi_main_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new MainPage());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }
    }
}
