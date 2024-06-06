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
                //create case
                if(temp.Data == null)
                {
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
                else
                {
                    //update case
                    var record = temp.Data as Record;
                    record.RecordId = Int32.Parse(RecordId.Text);
                    record.ClinicId = Int32.Parse(RecordClinicId.Text);
                    record.CustomerId = Int32.Parse(RecordCustomerId.Text);
                    record.NumOfVisits = Int32.Parse(NumOfVisits.Text);

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

                    var result = await _recordBusiness.Update(record);
                    MessageBox.Show(result.Message, "Update");

                    //reset text box
                    RecordId.Text = string.Empty;
                    RecordClinicId.Text = string.Empty;
                    RecordCustomerId.Text = string.Empty;
                    NumOfVisits.Text = string.Empty;
                    LoadRecords();
                }

                
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

        private async void grdRecord_MouseDouble_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Double Click on Grid");
            DataGrid grd = sender as DataGrid;
            if(grd != null && grd.SelectedItems != null && grd.SelectedItems.Count == 1)
            {
                var row = grd.ItemContainerGenerator.ContainerFromItem(grd.SelectedItem) as DataGridRow;
                if(row != null)
                {
                    var item = row.Item as Record;
                    if(item != null)
                    {
                        var recordResult = await _recordBusiness.GetById(item.RecordId);

                        if(recordResult.Status > 0 && recordResult.Data != null)
                        {
                            item = recordResult.Data as Record;
                            RecordId.Text = item.RecordId.ToString();
                            RecordClinicId.Text = item.ClinicId.ToString();
                            RecordCustomerId.Text = item.CustomerId.ToString();
                            NumOfVisits.Text = item.NumOfVisits.ToString();
                        }
                    }
                }
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            RecordId.Text = string.Empty;
            RecordClinicId.Text = string.Empty;
            RecordCustomerId.Text = string.Empty;
            NumOfVisits.Text = string.Empty;
        }

    }
}

