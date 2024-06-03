using Clinic.Business.Clinic;
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

namespace Clinic.WpfApp.UI
{
    /// <summary>
    /// Interaction logic for WRecord.xaml
    /// </summary>
    public partial class WRecord : Window
    {
        private readonly IRecordBusiness _recordBusiness;
        private readonly IClinicBusiness _clinicBusiness;
        private readonly ICustomerBusiness _customerBusiness;
        public WRecord()
        {
            InitializeComponent();
            _recordBusiness = new RecordBusiness();
            _clinicBusiness = new ClinicBusiness();
            _customerBusiness = new CustomerBusiness();
            LoadRecords();
        }

        private async void LoadRecords()
        {
            try
            {
                var records = await _recordBusiness.GetAll();
                if(records.Status > 0 && records.Data != null)
                {
                    recordList.ItemsSource = records.Data as List<Record>;
                }
                else
                {
                    recordList.ItemsSource = new List<Record>();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }


        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var temp = await _recordBusiness.GetById(Int32.Parse(RecordId.Text));
                if(temp.Data != null)
                {
                    MessageBox.Show("Record Id already exists");
                    return;
                }
                temp = await _clinicBusiness.GetById(Int32.Parse(RecordClinicId.Text));
                if(temp.Data == null)
                {
                    MessageBox.Show("Clinic Id not found");
                    return;
                }
                temp = await _customerBusiness.GetById(Int32.Parse(RecordCustomerId.Text));
                if(temp.Data == null)
                {
                    MessageBox.Show("Customer Id not found");
                    return;
                }

                var record = new Record()
                {
                    RecordId = Int32.Parse(RecordId.Text),
                    ClinicId = Int32.Parse(RecordClinicId.Text),
                    CustomerId = Int32.Parse(RecordCustomerId.Text),
                    NumOfVisits = Int32.Parse(NumOfVisits.Text)
                };
                
                var result = await _recordBusiness.Save(record);
                MessageBox.Show(result.Message, "Save");

                //reset text box
                RecordId.Text = string.Empty;
                RecordClinicId.Text = string.Empty;
                RecordCustomerId.Text = string.Empty;
                NumOfVisits.Text = string.Empty;

                //refresh list
                LoadRecords();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if(button == null)
                {
                    MessageBox.Show("Null button", "Error");
                    return;
                }
                int recordId = (int)button.CommandParameter;
                MessageBoxResult confirm = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(confirm == MessageBoxResult.Yes)
                {
                    var result = await _recordBusiness.DeleteById(recordId);
                    MessageBox.Show(result.Message, "Delete");
                }
                //refresh list
                LoadRecords();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if(button != null)
                {
                    int recordId = (int)button.CommandParameter;

                    var existingRecord = await _recordBusiness.GetById(recordId);
                    var record = existingRecord.Data as Record;

                    if(record != null)
                    {
                        RecordId.Text = record.RecordId.ToString();
                        RecordClinicId.Text = record.ClinicId.ToString();
                        RecordCustomerId.Text = record.CustomerId.ToString();
                        NumOfVisits.Text = record.NumOfVisits.ToString();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            RecordId.Text = string.Empty;
            RecordClinicId.Text = string.Empty;
            RecordCustomerId.Text = string.Empty;
            NumOfVisits.Text = string.Empty;
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var recordUpdate = new Record()
                {
                    RecordId = Int32.Parse(RecordId.Text),
                    ClinicId = Int32.Parse(RecordClinicId.Text),
                    CustomerId = Int32.Parse(RecordCustomerId.Text),
                    NumOfVisits = Int32.Parse(NumOfVisits.Text)
                };

                var existingRecord = await _recordBusiness.GetById(recordUpdate.RecordId);
                var record = existingRecord.Data as Record;
                if(existingRecord.Data == null)
                {
                    MessageBox.Show("Record ID doesn't exist", "Warning");
                    return;
                }
                var temp = await _clinicBusiness.GetById(recordUpdate.ClinicId);
                if(temp.Data == null)
                {
                    MessageBox.Show("Clinic ID doesn't exist", "Warning");
                    return;
                }
                temp = await _customerBusiness.GetById(recordUpdate.CustomerId);
                if(temp.Data == null)
                {
                    MessageBox.Show("Customer ID doesn't exist", "Warning");
                    return;
                }

                //update
                record.RecordId = recordUpdate.RecordId;
                record.ClinicId = recordUpdate.ClinicId;
                record.CustomerId = recordUpdate.CustomerId;
                record.NumOfVisits = recordUpdate.NumOfVisits;

                var result = await _recordBusiness.Update(record);
                MessageBox.Show(result.Message, "Update");

                //reset text box
                RecordId.Text = string.Empty;
                RecordClinicId.Text = string.Empty;
                RecordCustomerId.Text = string.Empty;
                NumOfVisits.Text = string.Empty;
                LoadRecords();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
        }
    }
}

