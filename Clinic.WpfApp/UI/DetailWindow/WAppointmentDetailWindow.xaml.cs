using Clinic.Business.Clinic;
using Clinic.Business;
using Clinic.Data.Models;
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

namespace Clinic.WpfApp.UI.DetailWindow
{
    /// <summary>
    /// Interaction logic for WAppointmentDetailWindow.xaml
    /// </summary>
    public partial class WAppointmentDetailWindow : Window
    {
        public WAppointmentDetailWindow(Data.Models.AppointmentDetail selectedAppointmentDetail)
        {
            InitializeComponent();
            this.DataContext = selectedAppointmentDetail;
        }

        //public WAppointmentDetailWindow()
        //{
        //    InitializeComponent();
        //}
    }
}
