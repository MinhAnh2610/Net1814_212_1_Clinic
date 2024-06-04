using Clinic.Business;
using Clinic.Data.Models;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Clinic.WpfApp.UI
{
    /// <summary>
    /// Interaction logic for wAppointment.xaml
    /// </summary>
    public partial class wAppointment : Window
    {
        private readonly IAppointmentBusiness _business;
        public wAppointment()
        {
            InitializeComponent();
            _business = new AppointmentBusiness();

        }
        private async void List_Loaded(object sender, RoutedEventArgs e)
        {
            // Call your async startup function
            await ListLoader();
        }
        private void NumberValidation(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(CustomerID.Text, "[^0-9]"))
                {
                    MessageBox.Show("Please enter only numbers in Customer ID .");
                    CustomerID.Text = CustomerID.Text.Remove(CustomerID.Text.Length - 1);
                }
                if (System.Text.RegularExpressions.Regex.IsMatch(Total.Text, "[^0-9.]"))
                {
                    MessageBox.Show("Please enter only numbers in Total .");
                    Total.Text = Total.Text.Remove(Total.Text.Length - 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
        private async Task ListLoader()
        {
            try
            {
                var appointments = await _business.GetAll();
                var result = appointments.Data as List<Appointment>;
                if (appointments.Status > 0 && appointments != null)
                {

                    w_appointments.ItemsSource = result;
                }
                else
                {
                    w_appointments.ItemsSource = new List<Appointment>();
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button != null)
                {
                    var selectedRow = w_appointments.SelectedItem;
                    var appointment = selectedRow as Appointment;
                    if (selectedRow != null)
                    {

                        var result =
                            await _business.DeleteById(appointment.AppointmentId);
                        MessageBox.Show(result.Message, "Deleted");
                        List_Loaded(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("Please select a row to delete", "Warning");
                    }


                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }

        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = await _business.GetById(Convert.ToInt32(AppointmentID.Text));

                if (item.Data == null)
                {
                    var appointment = new Appointment();
                    appointment.AppointmentId = Convert.ToInt32(AppointmentID.Text);
                        appointment.CustomerId = Convert.ToInt32(CustomerID.Text);
                        DateTime date = Convert.ToDateTime(Date.SelectedDate);
                        appointment.Date = DateOnly.FromDateTime(date);
                        appointment.Total = Convert.ToDecimal(Total.Text);
                        appointment.PaymentMethod = PaymentMethod.Text;
                        appointment.PaymentStatus = Convert.ToBoolean(PaymentStatus.IsChecked);
                        appointment.DentistName = DentistName.Text;
                   

                    var result = await _business.Save(appointment);
                    MessageBox.Show(result.Message, "Save");
                    await ListLoader();
                }
                else
                {
                    var appointment = item.Data as Appointment;
                    appointment.CustomerId = Convert.ToInt32(CustomerID.Text);
                    DateTime date = Convert.ToDateTime(Date.SelectedDate);
                    appointment.Date = DateOnly.FromDateTime(date);
                    appointment.Total = Convert.ToDecimal(Total.Text);
                    appointment.PaymentMethod = PaymentMethod.Text;
                    appointment.PaymentStatus = Convert.ToBoolean(PaymentStatus.IsChecked);
                    appointment.DentistName = DentistName.Text;

                    var result = await _business.Update(appointment);
                    MessageBox.Show(result.Message, "Update");
                    await ListLoader();
                }

                CustomerID.Text = string.Empty;
                Date.SelectedDate = null;
                Total.Text = string.Empty;
                PaymentMethod.Text = string.Empty;
                PaymentStatus.IsChecked = false;
                DentistName.Text = string.Empty;

                // Empty all textboxes
                CustomerID.Text = string.Empty;
                Date.SelectedDate = null;
                Total.Text = string.Empty;
                PaymentMethod.Text = string.Empty;
                PaymentStatus.IsChecked = false;
                DentistName.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }
        private async void DatagridUpdate(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit)
                {
                    var column = e.Column as DataGridBoundColumn;
                    var editedRow = e.Row.Item as Appointment;
                    var newCellContent = (e.EditingElement as TextBox).Text; // Get new value

                    // Determine which property to update
                    if (column.Header.ToString() == "Customer ID")
                    {
                        editedRow.CustomerId = Convert.ToInt32(newCellContent);
                    }
                    else if (column.Header.ToString() == "Total")
                    {
                        editedRow.Total = Convert.ToDecimal(newCellContent);
                    }
                    else if (column.Header.ToString() == "Payment Method")
                    {
                        editedRow.PaymentMethod = newCellContent;
                    }
                    else if (column.Header.ToString() == "Payment Status")
                    {
                        editedRow.PaymentStatus = Convert.ToBoolean(newCellContent);
                    }
                    else if (column.Header.ToString() == "Dentist Name")
                    {
                        editedRow.DentistName = newCellContent;
                    }
                    var result = await _business.Update(editedRow);
                    MessageBox.Show(result.Message, "Updated");
                    await ListLoader();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }



        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void grdAppointment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void w_appointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private async void grdAppointment_MouseDouble_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Double Click on Grid");
            DataGrid grd = sender as DataGrid;
            if (grd != null && grd.SelectedItems != null && grd.SelectedItems.Count == 1)
            {
                var row = grd.ItemContainerGenerator.ContainerFromItem(grd.SelectedItem) as DataGridRow;
                if (row != null)
                {
                    var item = row.Item as Appointment;
                    if (item != null)
                    {
                        var currencyResult = await _business.GetById(item.AppointmentId);

                        if (currencyResult.Status > 0 && currencyResult.Data != null)
                        {
                            item = currencyResult.Data as Appointment;
                            if (item != null)
                            {
                                AppointmentID.Text = item.AppointmentId.ToString();
                                CustomerID.Text =item.CustomerId.ToString();
                                Date.Text= item.Date.ToString();
                                Total.Text = item.Total.ToString();
                                PaymentMethod.Text = item.PaymentMethod;
                                PaymentStatus.IsChecked = item.PaymentStatus;
                                DentistName.Text = item.DentistName;
                            }
                        }
                    }
                }
            }

        }
    }
}
