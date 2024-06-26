﻿using Clinic.Business;
using Clinic.Business.Clinic;
using Clinic.Data.Models;
using Clinic.WpfApp.UI.DetailWindow;
using Clinic.WpfApp.UI.SearchWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace Clinic.WpfApp.UI
{
    /// <summary>
    /// Interaction logic for WRecordDetail.xaml
    /// </summary>
    public partial class WRecordDetail : Window
    {
        private readonly IRecordDetailBusiness _recordDetailBusiness;
        private readonly IRecordBusiness _recordBusiness;
        private readonly IAppointmentDetailBusiness _appointmentDetailBusiness;
        private readonly IAppointmentBusiness _appointmentBusiness;
        public WRecordDetail()
        {
            InitializeComponent();
            _recordDetailBusiness = new RecordDetailBusiness();
            _recordBusiness = new RecordBusiness();
            _appointmentDetailBusiness = new AppointmentDetailBusiness();
            _appointmentBusiness = new AppointmentBusiness();
            LoadRecordDetails();    
        }

        private async void LoadRecordDetails()
        {
            var recordDetails = await _recordDetailBusiness.GetAll();
            if (recordDetails.Status > 0 && recordDetails.Data != null)
            {
                recordDetailList.ItemsSource = recordDetails.Data as List<Data.Models.RecordDetail>;
            }
            else
            {
                recordDetailList.ItemsSource = new List<Data.Models.RecordDetail>();
            }
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var appointmentDetail = await _appointmentDetailBusiness.GetById(Int32.Parse(AppointmentDetailId.Text));
                if (appointmentDetail.Data == null) 
                {
                    MessageBox.Show("Appointment Detail ID doesn't exist", "Warning");
                    return;
                }

                var record = await _recordBusiness.GetById(Int32.Parse(RecordId.Text));
                if (record.Data == null)
                {
                    MessageBox.Show("Record ID doesn't exist", "Warning");
                    return;
                }

                var existingRecordDetail = await _recordDetailBusiness.GetById(Int32.Parse(RecordDetailId.Text));
                if (existingRecordDetail.Data != null)
                {
                    var recordDetailModel = existingRecordDetail.Data as Data.Models.RecordDetail;
                    var recordDetailUpdate = new Data.Models.RecordDetail()
                    {
                        RecordDetailId = Int32.Parse(RecordDetailId.Text),
                        AppointmentDetailId = Int32.Parse(AppointmentDetailId.Text),
                        RecordId = Int32.Parse(RecordId.Text),
                        Evaluation = Evaluation.Text,
                        Reccommend = Reccommend.Text,
                        Diagnosis = Diagnosis.Text,
                        Prescriptions = Prescriptions.Text,
                        Symptoms = Symptoms.Text,
                        TreatmentPlan = TreatmentPlan.Text,
                        Notes = Notes.Text,
                    };

                    recordDetailModel.Evaluation = recordDetailUpdate.Evaluation;
                    recordDetailModel.Reccommend = recordDetailUpdate.Reccommend;
                    recordDetailModel.Diagnosis = recordDetailUpdate.Diagnosis;
                    recordDetailModel.Prescriptions = recordDetailUpdate.Prescriptions;
                    recordDetailModel.Symptoms = recordDetailUpdate.Symptoms;
                    recordDetailModel.TreatmentPlan = recordDetailUpdate.TreatmentPlan;
                    recordDetailModel.Notes = recordDetailUpdate.Notes;

                    var result = await _recordDetailBusiness.Update(recordDetailModel);
                    MessageBox.Show(result.Message, "Update");

                    ButtonCancel_Click(sender, e);

                    LoadRecordDetails();
                }
                else
                {
                    var recordDetail = new Data.Models.RecordDetail()
                    {
                        RecordDetailId = Int32.Parse(RecordDetailId.Text),
                        AppointmentDetailId = Int32.Parse(AppointmentDetailId.Text),
                        RecordId = Int32.Parse(RecordId.Text),
                        Evaluation = Evaluation.Text,
                        Reccommend = Reccommend.Text,
                        Diagnosis = Diagnosis.Text,
                        Prescriptions = Prescriptions.Text,
                        Symptoms = Symptoms.Text,
                        TreatmentPlan = TreatmentPlan.Text,
                        Notes = Notes.Text,
                    };
                    var existingRecordDetails = await _recordDetailBusiness.GetAll();
                    if (existingRecordDetails.Data != null)
                    {
                        foreach (var item in (List<RecordDetail>)existingRecordDetails.Data)
                        {
                            if (item.AppointmentDetailId == recordDetail.AppointmentDetailId)
                            {
                                MessageBox.Show("Appointment Detail already has a Record Detail", "Warning");
                                return;
                            }
                        }
                    }
                    // Get the record
                    var existingRecord = record.Data as Record;
                    // Get the appointment from appointmentDetail
                    var existingAppointmentDetail = appointmentDetail.Data as AppointmentDetail;
                    var appointment = await _appointmentBusiness.GetById(existingAppointmentDetail!.AppointmentId);
                    var existingAppointment = appointment.Data as Appointment;
                    if (existingAppointment!.CustomerId != existingRecord!.CustomerId)
                    {
                        MessageBox.Show("This Record ID is invalid as it does not belong to this customer of the appointment", "Warning");
                        return;
                    }

                    var result = await _recordDetailBusiness.Save(recordDetail);
                    MessageBox.Show(result.Message, "Save");

                    ButtonCancel_Click(sender, e);

                    LoadRecordDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            RecordDetailId.Text = string.Empty;
            AppointmentDetailId.Text = string.Empty;
            RecordId.Text = string.Empty;
            Evaluation.Text = string.Empty;
            Reccommend.Text = string.Empty;
            Diagnosis.Text = string.Empty;
            Prescriptions.Text = string.Empty;
            Symptoms.Text = string.Empty;
            TreatmentPlan.Text = string.Empty;
            Notes.Text = string.Empty;

            RecordDetailId.IsEnabled = true;
            AppointmentDetailId.IsEnabled = true;
            RecordId.IsEnabled = true;
        }

        private void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var selectedRecordDetail = button?.DataContext as RecordDetail;

                if (selectedRecordDetail != null)
                {
                    var recordDetailWindow = new WRecordDetailWindow(selectedRecordDetail);
                    recordDetailWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button != null)
                {
                    int recordDetailId = (int)button.CommandParameter;

                    // Show confirmation message box
                    MessageBoxResult confirm = MessageBox.Show("Are you sure you want to delete this record detail?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (confirm == MessageBoxResult.Yes)
                    {
                        var result = await _recordDetailBusiness.DeleteById(recordDetailId);
                        MessageBox.Show(result.Message, "Delete");
                    }

                    // Update the form fields with the recordDetail details
                    LoadRecordDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            var p = new WRecordDetailSearch();
            p.Show();
        }
        private async void RecordDetail_MouseDouble_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Double Click on Grid");
            DataGrid recordDetail = sender as DataGrid;
            if (recordDetail != null && recordDetail.SelectedItems != null && recordDetail.SelectedItems.Count == 1)
            {
                var row = recordDetail.ItemContainerGenerator.ContainerFromItem(recordDetail.SelectedItem) as DataGridRow;
                if (row != null)
                {
                    var item = row.Item as Data.Models.RecordDetail;
                    if (item != null)
                    {
                        var recordDetailResult = await _recordDetailBusiness.GetById(item.RecordDetailId);

                        if (recordDetailResult.Status > 0 && recordDetailResult.Data != null)
                        {
                            item = recordDetailResult.Data as Data.Models.RecordDetail;
                            RecordDetailId.Text = item.RecordDetailId.ToString();
                            AppointmentDetailId.Text = item.AppointmentDetailId.ToString();
                            RecordId.Text = item.RecordId.ToString();
                            Evaluation.Text = item.Evaluation;
                            Reccommend.Text = item.Reccommend;
                            Diagnosis.Text = item.Diagnosis;
                            Prescriptions.Text = item.Prescriptions;
                            Symptoms.Text = item.Symptoms;
                            TreatmentPlan.Text = item.TreatmentPlan;
                            Notes.Text = item.Notes;

                            RecordDetailId.IsEnabled = false;
                            AppointmentDetailId.IsEnabled = false;
                            RecordId.IsEnabled = false;
                        }
                    }
                }
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(RecordDetailId.Text))
            {
                tbPlaceholder1.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder1.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(AppointmentDetailId.Text))
            {
                tbPlaceholder2.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder2.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(RecordId.Text))
            {
                tbPlaceholder3.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder3.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Evaluation.Text))
            {
                tbPlaceholder4.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder4.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Reccommend.Text))
            {
                tbPlaceholder5.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder5.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Diagnosis.Text))
            {
                tbPlaceholder6.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder6.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Prescriptions.Text))
            {
                tbPlaceholder7.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder7.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Symptoms.Text))
            {
                tbPlaceholder8.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder8.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(TreatmentPlan.Text))
            {
                tbPlaceholder9.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder9.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Notes.Text))
            {
                tbPlaceholder10.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder10.Visibility = Visibility.Hidden;
            }
        }
    }
}
