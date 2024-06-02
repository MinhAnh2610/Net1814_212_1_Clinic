using Clinic.Data.Models;
using Clinic.WpfApp.UI;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Clinic.WpfApp
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

        private void Open_wClinic_Click(object sender, RoutedEventArgs e)
        {
            var p = new WClinic();
            p.Owner = this;
            p.Show();
        }
        private void Open_wRecordDetail_Click(object sender, RoutedEventArgs e)
        {
            var p = new WRecordDetail();
            p.Owner = this;
            p.Show();
        }

        private void Open_WCustomer_Click(object sender, RoutedEventArgs e)
        {
            var p = new WCustomer();
            p.Owner = this;
            p.Show();
        }

        //private void Open_WRecord_Click(object sender, RoutedEventArgs e)
        //{
        //    var p = new WRecord();
        //    p.Owner = this;
        //    p.Show();
        //}
    }
}