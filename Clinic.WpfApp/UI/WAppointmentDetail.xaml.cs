using Clinic.Business;
using Clinic.Business.Base;
using Clinic.Business.Clinic;
using Clinic.Data.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Clinic.WpfApp.UI
{
    /// <summary>
    /// Interaction logic for WClinic.xaml
    /// </summary>
    public partial class WAppointmentDetail : Window
    {
        private readonly IAppointmentDetailBusiness _appointmentDetailBusiness;
        private readonly IAppointmentBusiness _appointmentBusiness;
        private readonly IServiceBusiness _serviceBusiness;
        public WAppointmentDetail()
        {
            InitializeComponent();
            _appointmentDetailBusiness = new AppointmentDetailBusiness();
            /*_appointmentBusiness = new AppointmentBusiness();
            _serviceBusiness = new ServiceBusiness();*/
            GetAllData();
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var AppointmentDetail = new Data.Models.AppointmentDetail()
                {
                    AppointmentDetailId = Int32.Parse(AppointmentDetailId.Text),
                    AppointmentId = Int32.Parse(AppointmentId.Text),
                    ServiceId = Int32.Parse(ServiceId.Text),
                    IsPeriodic = Boolean.Parse(isPeriodic.Text),
                    Day = Int32.Parse(Day.Text),
                    Month = Int32.Parse(Month.Text),
                    Year = Int32.Parse(Year.Text),
                };

                // TODO Validate Appointment & Service if these 2 null then return error
                var temp = await _appointmentDetailBusiness.GetById(AppointmentDetail.AppointmentDetailId);
                if(temp.Data != null)
                {
                    System.Windows.MessageBox.Show("Appointment Detail ID already exists");
                    return;
                }
                temp = await _appointmentBusiness.GetById(AppointmentDetail.AppointmentId);
                if (temp.Data == null)
                {
                    System.Windows.MessageBox.Show("Appointment Id not found");
                    return;
                }
                temp = await _serviceBusiness.GetById(AppointmentDetail.ServiceId);
                if (temp.Data == null)
                {
                    System.Windows.MessageBox.Show("Service Id not found");
                    return;
                }

                var result = await _appointmentDetailBusiness.Save(AppointmentDetail);
                System.Windows.MessageBox.Show(result.Message, "Save");

                AppointmentDetailId.Text = string.Empty;
                AppointmentId.Text = string.Empty;
                ServiceId.Text = string.Empty;
                isPeriodic.Text = string.Empty;
                Day.Text = string.Empty;
                Month.Text = string.Empty;
                Year.Text = string.Empty;
                GetAllData();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error");
            }

        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            AppointmentDetail appointmentDetail = AppointmentDetailList.SelectedItem as AppointmentDetail;
            if (appointmentDetail == null)
            {
                System.Windows.MessageBox.Show("AppointmentDetailID not found", "Warning");
                return;
            }

            var result = await _appointmentDetailBusiness.DeleteById(appointmentDetail.AppointmentDetailId);
            System.Windows.MessageBox.Show(result.Message, "Delete");
            GetAllData();

            try
            {
                var button = sender as System.Windows.Controls.Button;
                if (button == null)
                {
                    System.Windows.MessageBox.Show("Null button", "Error");
                    return;
                }
                int appointmentDetailId = (int)button.CommandParameter;
                MessageBoxResult confirm = System.Windows.MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    var apointmentDetail = await _appointmentDetailBusiness.DeleteById(appointmentDetailId);
                    System.Windows.MessageBox.Show(result.Message, "Delete");
                }
                //refresh list
                GetAllData();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            AppointmentDetailId.Text = string.Empty;
            AppointmentId.Text = string.Empty;
            ServiceId.Text = string.Empty;
            isPeriodic.Text = string.Empty;
            Day.Text = string.Empty;
            Month.Text = string.Empty;
            Year.Text = string.Empty;
        }

        private async void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as System.Windows.Controls.Button;
                if (button != null)
                {
                    int appointmentDetailId = (int)button.CommandParameter;

                    var existingAppointmentDetail = await _appointmentDetailBusiness.GetById(appointmentDetailId);
                    var appointmentDetail = existingAppointmentDetail.Data as AppointmentDetail;

                    if (appointmentDetail != null)
                    {
                        AppointmentDetailId.Text = appointmentDetail.AppointmentDetailId.ToString();
                        AppointmentId.Text = appointmentDetail.AppointmentId.ToString();
                        ServiceId.Text = appointmentDetail.ServiceId.ToString();
                        isPeriodic.Text = appointmentDetail.IsPeriodic.ToString();
                        Day.Text = appointmentDetail.Day.ToString();
                        Month.Text = appointmentDetail.Month.ToString();
                        Year.Text = appointmentDetail.Year.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}", "Error");
            }
        }

        private async void GetAllData()
        {
            var result = await _appointmentDetailBusiness.GetAll();

            if (result.Status > 0 && result.Data != null)
            {
                AppointmentDetailList.ItemsSource = result.Data as List<AppointmentDetail>;
            }
            else
            {
                AppointmentDetailList.ItemsSource = new List<AppointmentDetail>();
            }
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var appointmentDetailUpdate = new AppointmentDetail()
                {
                    AppointmentDetailId = Int32.Parse(AppointmentDetailId.Text),
                    AppointmentId = Int32.Parse(AppointmentId.Text),
                    ServiceId = Int32.Parse(ServiceId.Text),
                    IsPeriodic = Boolean.Parse(isPeriodic.Text),
                    Day = Int32.Parse(Day.Text),
                    Month = Int32.Parse(Month.Text),
                    Year = Int32.Parse(Year.Text),
                };

                var existingAppointmentDetail = await _appointmentDetailBusiness.GetById(appointmentDetailUpdate.AppointmentDetailId);
                var appointmentDetail = existingAppointmentDetail.Data as AppointmentDetail;
                if (existingAppointmentDetail.Data == null)
                {
                    System.Windows.MessageBox.Show("Appointemnt Detail ID doesn't exist", "Warning");
                    return;
                }
                var temp = await _appointmentDetailBusiness.GetById(appointmentDetailUpdate.AppointmentId);
                if (temp.Data == null)
                {
                    System.Windows.MessageBox.Show("Appointment ID doesn't exist", "Warning");
                    return;
                }
                temp = await _appointmentDetailBusiness.GetById(appointmentDetailUpdate.ServiceId);
                if (temp.Data == null)
                {
                    System.Windows.MessageBox.Show("Service ID doesn't exist", "Warning");
                    return;
                }

                temp = await _appointmentDetailBusiness.GetById(appointmentDetailUpdate.AppointmentDetailId);
                if (temp.Data != null)
                {
                    System.Windows.MessageBox.Show("Appointment ID doesn't exist\", \"Warning");
                    return;
                }

                //update
                appointmentDetail.AppointmentDetailId = appointmentDetailUpdate.AppointmentDetailId;
                appointmentDetail.AppointmentId = appointmentDetailUpdate.AppointmentId;
                appointmentDetail.ServiceId = appointmentDetailUpdate.ServiceId;
                appointmentDetail.IsPeriodic = appointmentDetailUpdate.IsPeriodic;
                appointmentDetail.Day = appointmentDetailUpdate.Day;
                appointmentDetail.Month = appointmentDetailUpdate.Month;
                appointmentDetail.Year = appointmentDetailUpdate.Year;

                var result = await _appointmentDetailBusiness.Update(appointmentDetail);
                System.Windows.MessageBox.Show(result.Message, "Update");

                //reset text box
                AppointmentDetailId.Text = string.Empty;
                AppointmentId.Text = string.Empty;
                ServiceId.Text = string.Empty;
                isPeriodic.Text = string.Empty;
                Day.Text = string.Empty;
                Month.Text = string.Empty;
                Year.Text = string.Empty;
                GetAllData();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"{ex.Message}", "Error");
            }
        }
    }
}
