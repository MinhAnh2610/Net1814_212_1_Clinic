using Clinic.Business;
using Clinic.Business.Clinic;
using Clinic.Data.Models;
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
        public WRecordDetail()
        {
            InitializeComponent();
            _recordDetailBusiness = new RecordDetailBusiness();
            _recordBusiness = new RecordBusiness();
            _appointmentDetailBusiness = new AppointmentDetailBusiness();
            LoadRecordDetails();    
        }

        private async void LoadRecordDetails()
        {
            var recordDetails = await _recordDetailBusiness.GetAll();
            if (recordDetails.Status > 0 && recordDetails.Data != null)
            {
                recordList.ItemsSource = recordDetails.Data as List<Data.Models.RecordDetail>;
            }
            else
            {
                recordList.ItemsSource = new List<Data.Models.RecordDetail>();
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
                    };

                    recordDetailModel.Evaluation = recordDetailUpdate.Evaluation;
                    recordDetailModel.Reccommend = recordDetailUpdate.Reccommend;

                    var result = await _recordDetailBusiness.Update(recordDetailModel);
                    MessageBox.Show(result.Message, "Update");

                    RecordDetailId.Text = string.Empty;
                    AppointmentDetailId.Text = string.Empty;
                    RecordId.Text = string.Empty;
                    Evaluation.Text = string.Empty;
                    Reccommend.Text = string.Empty;

                    RecordDetailId.IsEnabled = true;
                    AppointmentDetailId.IsEnabled = true;
                    RecordId.IsEnabled = true;

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

                    var result = await _recordDetailBusiness.Save(recordDetail);
                    MessageBox.Show(result.Message, "Save");

                    RecordDetailId.Text = string.Empty;
                    AppointmentDetailId.Text = string.Empty;
                    RecordId.Text = string.Empty;
                    Evaluation.Text = string.Empty;
                    Reccommend.Text = string.Empty;

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

            RecordDetailId.IsEnabled = true;
            AppointmentDetailId.IsEnabled = true;
            RecordId.IsEnabled = true;
        }

        private async void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button != null)
                {
                    int recordDetailId = (int)button.CommandParameter;

                    // Fetch the clinic details using the ClinicId
                    var existingRecordDetail = await _recordDetailBusiness.GetById(recordDetailId);
                    var recordDetailModel = existingRecordDetail.Data as Data.Models.RecordDetail;

                    // Update the form fields with the recordDetail details
                    if (recordDetailModel != null)
                    {
                        RecordDetailId.Text = recordDetailModel.RecordDetailId.ToString();
                        AppointmentDetailId.Text = recordDetailModel.AppointmentDetailId.ToString();
                        RecordId.Text = recordDetailModel.RecordId.ToString();
                        Evaluation.Text = recordDetailModel.Evaluation;
                        Reccommend.Text = recordDetailModel.Reccommend;

                        RecordDetailId.IsEnabled = false;
                        AppointmentDetailId.IsEnabled = false;
                        RecordId.IsEnabled = false;
                    }
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

                            RecordDetailId.IsEnabled = false;
                            AppointmentDetailId.IsEnabled = false;
                            RecordId.IsEnabled = false;
                        }
                    }
                }
            }
        }
    }
}
