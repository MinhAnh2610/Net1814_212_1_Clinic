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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.IdentityModel.Tokens;

namespace Clinic.WpfApp.UI
{
    /// <summary>
    /// Interaction logic for WRecord.xaml
    /// </summary>
    public partial class WRecord : Window
    {
        private IRecordBusiness _recordBusiness;
        private IClinicBusiness _clinicBusiness;
        private ICustomerBusiness _customerBusiness;
        private IRecordDetailBusiness _recordDetailBusiness;

        public WRecord()
        {
            InitializeComponent();
            _recordBusiness = new RecordBusiness();
            _clinicBusiness = new ClinicBusiness();
            _customerBusiness = new CustomerBusiness();
            _recordDetailBusiness = new RecordDetailBusiness();
            LoadRecords();
        }

        //###################  Load Records ##################
        private async void LoadRecords(IEnumerable<Record> records = null)
        {
            try
            {
                if(records is not null)
                {
                    recordList.ItemsSource = records;
                    return;
                }
                var br = await _recordBusiness.GetAll();
                records = br.Data as List<Record>;
                if(!records.IsNullOrEmpty())
                {
                    recordList.ItemsSource = records;
                }
                else
                {
                    recordList.ItemsSource = new List<Record>();
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
            }
        }

        //################### Save Button ##################
        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var temp = await _recordBusiness.GetById(Int32.Parse(RecordId.Text));
                //create case
                if(temp.Data == null)
                {
                    var record = new Record()
                    {
                        RecordId = Int32.Parse(RecordId.Text)


                    };
                    temp = await _clinicBusiness.GetById(Int32.Parse(RecordClinicId.Text));
                    if(temp.Data == null)
                    {
                        System.Windows.MessageBox.Show("Clinic Id not found");
                        return;
                    }

                    record.ClinicId = Int32.Parse(RecordClinicId.Text);

                    temp = await _customerBusiness.GetById(Int32.Parse(RecordCustomerId.Text));
                    if(temp.Data == null)
                    {
                        System.Windows.MessageBox.Show("Customer Id not found");
                        return;
                    }

                    record.CustomerId = Int32.Parse(RecordCustomerId.Text);

                    if(NumOfVisits.Text.IsNullOrEmpty())
                    {
                        record.NumOfVisits = 0;
                    }
                    else
                    {
                        record.NumOfVisits = Int32.Parse(NumOfVisits.Text);
                    }


                    var result = await _recordBusiness.Save(record);
                    System.Windows.MessageBox.Show(result.Message, "Save");
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
                        System.Windows.MessageBox.Show("Clinic Id not found");
                        return;
                    }
                    temp = await _customerBusiness.GetById(Int32.Parse(RecordCustomerId.Text));
                    if(temp.Data == null)
                    {
                        System.Windows.MessageBox.Show("Customer Id not found");
                        return;
                    }

                    var result = await _recordBusiness.Update(record);
                    System.Windows.MessageBox.Show(result.Message, "Update");



                    //reset text box
                    RecordId.Text = string.Empty;
                    RecordClinicId.Text = string.Empty;
                    RecordCustomerId.Text = string.Empty;
                    NumOfVisits.Text = string.Empty;
                    LoadRecords();
                }
            }
            catch(FormatException fe)
            {
                System.Windows.MessageBox.Show("Wrong input format", "Invalid input");
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error");
            }

        }

        //################### Delete Button ##################
        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as System.Windows.Controls.Button;
                if(button == null)
                {
                    System.Windows.MessageBox.Show("Null button", "Error");
                    return;
                }
                int recordId = (int)button.CommandParameter;
                MessageBoxResult confirm = System.Windows.MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if(confirm == MessageBoxResult.Yes)
                {
                    var result = await _recordBusiness.DeleteById(recordId);
                    System.Windows.MessageBox.Show(result.Message, "Delete");
                }
                //refresh list
                LoadRecords();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
            }
        }

        //################### Mouse double click ##################
        private async void grdRecord_MouseDouble_Click(object sender, RoutedEventArgs e)
        {
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

        //################### Cancel Button ##################
        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            RecordId.Text = string.Empty;
            RecordClinicId.Text = string.Empty;
            RecordCustomerId.Text = string.Empty;
            NumOfVisits.Text = string.Empty;
        }

        //################### Text change on hover ##################
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(RecordId.Text))
            {
                tbPlaceholder1.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder1.Visibility = Visibility.Hidden;
            }

            if(string.IsNullOrEmpty(RecordClinicId.Text))
            {
                tbPlaceholder2.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder2.Visibility = Visibility.Hidden;
            }

            if(string.IsNullOrEmpty(RecordCustomerId.Text))
            {
                tbPlaceholder3.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder3.Visibility = Visibility.Hidden;
            }

            if(string.IsNullOrEmpty(NumOfVisits.Text))
            {
                tbPlaceholder4.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder4.Visibility = Visibility.Hidden;
            }
        }

        private async void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_recordBusiness is null)
            {
                _recordBusiness = new RecordBusiness();
            }
            if(cbSort.SelectedItem != null)
            {
                string selectedOption = (cbSort.SelectedItem as ComboBoxItem).Content.ToString();
                var br = await _recordBusiness.GetAll();
                var list = br.Data as List<Record>;
                IEnumerable<Record> sortedList = new List<Record>();
                switch(selectedOption)
                {
                    case "Sort by Id":
                        sortedList = list;
                        break;
                    case "Sort by Num of visits":
                        sortedList = list.OrderBy(r => r.NumOfVisits);
                        break;
                }
                LoadRecords(sortedList);
            }
        }

        private async void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            string recordId = RecordId.Text.Trim();
            string clinicId = RecordClinicId.Text.Trim();
            string customerId = RecordCustomerId.Text.Trim();
            string numOfVisit = NumOfVisits.Text.Trim();
            var bResult = await _recordBusiness.GetAll();
            var records = bResult.Data as List<Record>;

            var query = from record in records
                        where (string.IsNullOrEmpty(recordId) || record.RecordId.ToString() == recordId)
                              && (string.IsNullOrEmpty(clinicId) || record.ClinicId.ToString() == clinicId)
                              && (string.IsNullOrEmpty(customerId) || record.CustomerId.ToString() == customerId)
                              && (string.IsNullOrEmpty(numOfVisit) || record.NumOfVisits.ToString() == numOfVisit)
                        select record;

            LoadRecords(query.ToList());
        }
    }
}

