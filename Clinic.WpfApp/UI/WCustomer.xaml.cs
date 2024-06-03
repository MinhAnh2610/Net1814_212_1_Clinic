using Clinic.Business.Base;
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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WCustomer : Window
    {
        private readonly ICustomerBusiness _customerBusiness;
        public WCustomer()
        {
            InitializeComponent();
            _customerBusiness = new CustomerBusiness();
            LoadCustomers();
        }

        private async void LoadCustomers()
        {
            try
            {
                var customers = await _customerBusiness.GetAll();
                if(customers.Status > 0 && customers.Data != null)
                {
                    customerList.ItemsSource = customers.Data as List<Customer>;
                }
                else
                {
                    customerList.ItemsSource = new List<Customer>();
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
                //check if customer id already existed
                var customerBR = await _customerBusiness.GetById(Int32.Parse(CustomerId.Text));
                var existingCustomer = customerBR.Data as Customer;
                var isExist = existingCustomer != null;
                IBusinessResult result = new BusinessResult();
                var newCustomer = new Customer()
                {
                    CustomerId = Int32.Parse(CustomerId.Text),
                    FirstName = CustomerFirstName.Text,
                    LastName = CustomerLastName.Text,
                    Phone = CustomerPhone.Text
                };

                //case : update
                if(isExist)
                {
                    existingCustomer.CustomerId = newCustomer.CustomerId;
                    existingCustomer.FirstName = newCustomer.FirstName;
                    existingCustomer.LastName = newCustomer.LastName;
                    existingCustomer.Phone = newCustomer.Phone;

                    result = await _customerBusiness.Update(existingCustomer);
                    MessageBox.Show(result.Message, "Update");

                    //reset text box
                    CustomerId.Text = string.Empty;
                    CustomerFirstName.Text = string.Empty;
                    CustomerLastName.Text = string.Empty;
                    CustomerPhone.Text = string.Empty;

                    //refresh list
                    LoadCustomers();
                    return;
                }

                //case : create
                result = await _customerBusiness.Save(newCustomer);
                MessageBox.Show(result.Message, "Save");

                //reset text box
                CustomerId.Text = string.Empty;
                CustomerFirstName.Text = string.Empty;
                CustomerLastName.Text = string.Empty;
                CustomerPhone.Text = string.Empty;

                //refresh list
                LoadCustomers();
                return;

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
                //get id
                var button = sender as Button;
                if(button == null)
                {
                    MessageBox.Show("Null button", "Error");
                    return;
                }
                int customerId = (int)button.CommandParameter;

                //get confirmation
                MessageBoxResult confirm = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                //delete
                if(confirm == MessageBoxResult.Yes)
                {
                    var result = await _customerBusiness.DeleteById(customerId);
                    MessageBox.Show(result.Message, "Delete");
                }
                //refresh list
                LoadCustomers();
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
                //get id
                var button = sender as Button;
                if(button != null)
                {
                    int customerId = (int)button.CommandParameter;
                    var existingClinic = await _customerBusiness.GetById(customerId);
                    var customer = existingClinic.Data as Customer;

                    if(customer != null)
                    {
                        CustomerId.Text = customer.CustomerId.ToString();
                        CustomerFirstName.Text = customer.FirstName;
                        CustomerLastName.Text = customer.LastName;
                        CustomerPhone.Text = customer.Phone;
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
            //reset text box
            CustomerId.Text = string.Empty;
            CustomerFirstName.Text = string.Empty;
            CustomerLastName.Text = string.Empty;
            CustomerPhone.Text = string.Empty;
        }

        private async void grdCustomer_MouseDouble_Click(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("Double Click on Grid");
            DataGrid grd = sender as DataGrid;
            if(grd != null && grd.SelectedItems != null && grd.SelectedItems.Count == 1)
            {
                var row = grd.ItemContainerGenerator.ContainerFromItem(grd.SelectedItem) as DataGridRow;
                if(row != null)
                {
                    var item = row.Item as Customer;
                    if(item != null)
                    {
                        var currencyResult = await _customerBusiness.GetById(item.CustomerId);

                        if(currencyResult.Status > 0 && currencyResult.Data != null)
                        {
                            item = currencyResult.Data as Customer;
                            CustomerId.Text = item.CustomerId.ToString();
                            CustomerFirstName.Text = item.FirstName;
                            CustomerLastName.Text = item.LastName;
                            CustomerPhone.Text = item.Phone;
                        }
                    }
                }
            }
        }
    }
}
