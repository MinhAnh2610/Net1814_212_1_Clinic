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

        //private async void ButtonLoadList_Click(object sender, RoutedEventArgs e)
        //{
        //    LoadCustomers();
        //}

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var customer = new Customer()
                {
                    CustomerId = Int32.Parse(CustomerId.Text),
                    FirstName = CustomerFirstName.Text,
                    LastName = CustomerLastName.Text,
                    Phone = CustomerPhone.Text
                };
                var temp = await _customerBusiness.GetById(customer.CustomerId);
                if(temp.Data != null)
                {
                    MessageBox.Show("Customer Id already exists");
                    return;
                }

                var result = await _customerBusiness.Save(customer);
                MessageBox.Show(result.Message, "Save");

                CustomerId.Text = string.Empty;
                CustomerFirstName.Text = string.Empty;
                CustomerLastName.Text = string.Empty;
                CustomerPhone.Text = string.Empty;

                LoadCustomers();
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
                int customerId = (int)button.CommandParameter;
                MessageBoxResult confirm = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
            CustomerId.Text = string.Empty;
            CustomerFirstName.Text = string.Empty;
            CustomerLastName.Text = string.Empty;
            CustomerPhone.Text = string.Empty;
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var customerUpdate = new Customer()
                {
                    CustomerId = Int32.Parse(CustomerId.Text),
                    FirstName = CustomerFirstName.Text,
                    LastName = CustomerLastName.Text,
                    Phone = CustomerPhone.Text,
                };

                var existingCustomer = await _customerBusiness.GetById(customerUpdate.CustomerId);
                var customer = existingCustomer.Data as Customer;
                if(existingCustomer.Data == null)
                {
                    MessageBox.Show("Customer ID doesn't exist", "Warning");
                }
                else if(customer != null)
                {
                    customer.CustomerId = customerUpdate.CustomerId;
                    customer.FirstName = customerUpdate.FirstName;
                    customer.LastName = customerUpdate.LastName;
                    customer.Phone= customerUpdate.Phone;

                    var result = await _customerBusiness.Update(customer);
                    MessageBox.Show(result.Message, "Update");

                    //reset text box
                    CustomerId.Text = string.Empty;
                    CustomerFirstName.Text = string.Empty;
                    CustomerLastName.Text = string.Empty;
                    CustomerPhone.Text = string.Empty;
                    LoadCustomers();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
        }
    }
}
