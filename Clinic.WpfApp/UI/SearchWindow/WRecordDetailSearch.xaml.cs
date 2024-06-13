using Clinic.Business.Clinic;
using Clinic.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Clinic.WpfApp.UI.SearchWindow
{
    /// <summary>
    /// Interaction logic for WRecordDetailSearch.xaml
    /// </summary>
    public partial class WRecordDetailSearch : Window
    {
        private readonly IRecordDetailBusiness _recordDetailBusiness;
        public WRecordDetailSearch()
        {
            InitializeComponent();
            _recordDetailBusiness = new RecordDetailBusiness();
            LoadRecordDetails(null);
        }
        private async void LoadRecordDetails(List<Data.Models.RecordDetail> recordDetails)
        {
            try
            {
                if (recordDetails != null)
                {
                    recordDetailList.ItemsSource = recordDetails;
                }
                else
                {
                    var recordDetailListGet = await _recordDetailBusiness.GetAll();
                    recordDetailList.ItemsSource = recordDetailListGet.Data as List<Data.Models.RecordDetail>;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        private void ButtonAdd_Click(Object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ButtonCancel_Click(Object sender, RoutedEventArgs e)
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
        }
        private async void ButtonSearch_Click(Object sender, RoutedEventArgs e)
        {
            var recordDetailGetAll = await _recordDetailBusiness.GetAll();
            var recordDetailList = recordDetailGetAll.Data as List<Data.Models.RecordDetail>;

            string recordDetailId = RecordDetailId.Text;
            string appointmentDetailId = AppointmentDetailId.Text;
            string recordId = RecordId.Text;
            string evaluation = Evaluation.Text;
            string reccommend = Reccommend.Text;
            string diagnosis = Diagnosis.Text;
            string prescriptions = Prescriptions.Text;
            string symptoms = Symptoms.Text;
            string treatmentPlan = TreatmentPlan.Text;
            string notes = Notes.Text;

            // Filter the record details based on the input values
            var filteredRecordDetails = recordDetailList.Where(rd =>
                (string.IsNullOrEmpty(recordDetailId) || rd.RecordDetailId.ToString().Contains(recordDetailId, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(appointmentDetailId) || rd.AppointmentDetailId.ToString().Contains(appointmentDetailId, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(recordId) || rd.RecordId.ToString().Contains(recordId, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(evaluation) || rd.Evaluation.Contains(evaluation, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(reccommend) || rd.Reccommend.Contains(reccommend, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(diagnosis) || rd.Diagnosis.Contains(diagnosis, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(prescriptions) || rd.Prescriptions.Contains(prescriptions, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(symptoms) || rd.Symptoms.Contains(symptoms, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(treatmentPlan) || rd.TreatmentPlan.Contains(treatmentPlan, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(notes) || rd.Notes.Contains(notes, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            LoadRecordDetails(filteredRecordDetails);
        }
    }
}
