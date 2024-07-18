using Clinic.Business.Base;
using Clinic.Business.Clinic;
using Clinic.Data.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
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

namespace Clinic.WpfApp.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WCustomer : Window
    {
        private ICustomerBusiness _customerBusiness;
        public WCustomer()
        {
            InitializeComponent();
            _customerBusiness = new CustomerBusiness();
            ResizeMode = ResizeMode.CanResize;
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Maximized;
            LoadCustomers();
        }

        private async void LoadCustomers()
        {
            try
            {
                var customers = await _customerBusiness.GetAll();
                if(customers.Status > 0 && customers.Data != null)
                {
                    CustomerListDataGrid.ItemsSource = customers.Data as List<Customer>;
                }
                else
                {
                    CustomerListDataGrid.ItemsSource = new List<Customer>();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void LoadCustomers(IEnumerable<Customer> customers)
        {
            try
            {
                //if empty list, load default list
                if(customers.IsNullOrEmpty())
                {
                    LoadCustomers();
                    return;

                }
                CustomerListDataGrid.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            string customerId = CustomerId.Text.Trim();
            string firstName = CustomerFirstName.Text.Trim();
            string lastName = CustomerLastName.Text.Trim();
            string phone = CustomerPhone.Text.Trim();
            string address = CustomerAddress.Text.Trim();
            string email = CustomerEmail.Text.Trim();
            string dob = CustomerDoB.Text.Trim();
            string createdAt = CustomerCreatedAt.Text.Trim();
            var bResult = await _customerBusiness.GetAll();
            var customerList = bResult.Data as List<Customer>;
           
            // Create a query to filter the customers based on the search criteria
            var query = from customer in customerList
                        where (string.IsNullOrEmpty(customerId) || customer.CustomerId.ToString() == customerId)
                        && (string.IsNullOrEmpty(firstName) || customer.FirstName.Contains(firstName))
                        && (string.IsNullOrEmpty(lastName) || customer.LastName.Contains(lastName))
                        && (string.IsNullOrEmpty(phone) || customer.Phone.Contains(phone))
                        && (string.IsNullOrEmpty(address) || customer.Address.Contains(address))
                        && (string.IsNullOrEmpty(email) || customer.Email.Contains(email))
                        && (string.IsNullOrEmpty(dob) || customer.DoB.ToString() == dob)
                        && (string.IsNullOrEmpty(createdAt) || customer.CreatedAt.ToString() == createdAt)
                        select customer;

            // Bind the results to a data source (e.g., a DataGridView)
            LoadCustomers(query.ToList());
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //check if customer id already existed
                var item = await _customerBusiness.GetById(Int32.Parse(CustomerId.Text));

                
                //case : update
                if(item.Data != null)
                {
                    if((DateOnly.Parse(CustomerDoB.Text) > DateOnly.FromDateTime(DateTime.Now)) || (DateOnly.Parse(CustomerDoB.Text) < DateOnly.FromDateTime(DateTime.Now).AddYears(-120)))
                    {
                        MessageBox.Show("Invalid date of birth");
                        return;
                    }
                    ComboBoxItem comboBoxGender = (ComboBoxItem)comboGender.SelectedItem;
                    string gender = comboBoxGender.Content.ToString();
                    ComboBoxItem comboBoxIsActive = (ComboBoxItem)comboIsActive.SelectedItem;
                    string isActive = comboBoxIsActive.Content.ToString();
                    var customer = item.Data as Customer;
                    customer.CustomerId = Int32.Parse(CustomerId.Text);
                    customer.FirstName = CustomerFirstName.Text;
                    customer.LastName = CustomerLastName.Text;
                    customer.Phone = CustomerPhone.Text;
                    customer.Address = CustomerAddress.Text;
                    customer.Gender = (gender == "Male") ? true : false;
                    customer.Email = CustomerEmail.Text;
                    
                    customer.DoB = DateOnly.Parse(CustomerDoB.Text);
                    customer.CreatedAt = (CustomerCreatedAt.Text == string.Empty) ? DateOnly.FromDateTime(DateTime.Now) : DateOnly.Parse(CustomerCreatedAt.Text);
                    customer.IsActive = (isActive == "True") ? true : false;

                    var result = await _customerBusiness.Update(customer);
                    MessageBox.Show(result.Message, "Update");

                    //reset text box
                    RefreshTextBox();

                    //refresh list
                    LoadCustomers();
                }
                else
                {
                    if((DateOnly.Parse(CustomerDoB.Text) > DateOnly.FromDateTime(DateTime.Now)) || (DateOnly.Parse(CustomerDoB.Text) < DateOnly.FromDateTime(DateTime.Now).AddYears(-120)))
                    {
                        MessageBox.Show("Invalid date of birth");
                        return;
                    }
                    ComboBoxItem comboBoxGender = (ComboBoxItem)comboGender.SelectedItem;
                    string gender = comboBoxGender.Content.ToString();
                    ComboBoxItem comboBoxIsAtive = (ComboBoxItem)comboIsActive.SelectedItem;
                    string isActive = comboBoxIsAtive.Content.ToString();
                    var customer = new Customer()
                    {
                        CustomerId = Int32.Parse(CustomerId.Text),
                        FirstName = CustomerFirstName.Text,
                        LastName = CustomerLastName.Text,
                        Phone = CustomerPhone.Text,
                        Address = CustomerAddress.Text,
                        Gender = (gender == "Male") ? true : false,
                        Email = CustomerEmail.Text,
                        DoB = DateOnly.Parse(CustomerDoB.Text),
                        CreatedAt = (CustomerCreatedAt.Text == string.Empty) ? DateOnly.FromDateTime(DateTime.Now) : DateOnly.Parse(CustomerCreatedAt.Text),
                        IsActive = (isActive == "True") ? true : false
                    };

                    var result = await _customerBusiness.Save(customer);
                    MessageBox.Show(result.Message, "Save");

                    //reset text box
                    RefreshTextBox();

                    //refresh list
                    LoadCustomers();
                }
            }
            catch(FormatException fe)
            {
                MessageBox.Show("Wrong input format", "Invalid input");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void RefreshTextBox()
        {
            CustomerId.Text = string.Empty;
            CustomerFirstName.Text = string.Empty;
            CustomerLastName.Text = string.Empty;
            CustomerPhone.Text = string.Empty;
            CustomerAddress.Text = string.Empty;
            CustomerEmail.Text = string.Empty;
            CustomerDoB.Text = string.Empty;
            CustomerCreatedAt.Text = string.Empty;
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


        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            //reset text box
            RefreshTextBox();
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
                        var customerResult = await _customerBusiness.GetById(item.CustomerId);

                        if(customerResult.Status > 0 && customerResult.Data != null)
                        {
                            item = customerResult.Data as Customer;
                            CustomerId.Text = item.CustomerId.ToString();
                            CustomerFirstName.Text = item.FirstName;
                            CustomerLastName.Text = item.LastName;
                            CustomerPhone.Text = item.Phone;
                            CustomerAddress.Text = item.Address;
                            comboGender.SelectedIndex = (bool)(item.Gender) ? 0 : 1;
                            CustomerEmail.Text = item.Email;
                            CustomerDoB.Text = item.DoB.ToString();
                            CustomerCreatedAt.Text = item.CreatedAt.ToString();
                            comboIsActive.SelectedIndex = (bool)(item.IsActive) ? 0 : 1;
                        }
                    }
                }
            }
        }

        private async void Sort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_customerBusiness is null)
            {
                _customerBusiness = new CustomerBusiness();
            }
            if(cbSort.SelectedItem != null)
            {
                string selectedOption = (cbSort.SelectedItem as ComboBoxItem).Content.ToString();
                var br = await _customerBusiness.GetAll();
                var list = br.Data as List<Customer>;
                IEnumerable<Customer> sortedList = new List<Customer>();
                switch(selectedOption)
                {
                    case "Sort by Customer Id":
                        sortedList = list;
                        break;
                    case "Sort by Created Date":
                        sortedList = list.OrderBy(c => c.CreatedAt);
                        break;
                    case "Sort by Birthdate":
                        sortedList = list.OrderBy(c => c.DoB);
                        break;
                }
                LoadCustomers(sortedList);
            }
        }
        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CustomerId.Text))
            {
                tbPlaceholder1.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder1.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(CustomerFirstName.Text))
            {
                tbPlaceholder2.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder2.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(CustomerLastName.Text))
            {
                tbPlaceholder3.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder3.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(CustomerPhone.Text))
            {
                tbPlaceholder4.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder4.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(CustomerAddress.Text))
            {
                tbPlaceholder5.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder5.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(CustomerEmail.Text))
            {
                tbPlaceholder6.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder6.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(CustomerDoB.Text))
            {
                tbPlaceholder7.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder7.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(CustomerCreatedAt.Text))
            {
                tbPlaceholder8.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder8.Visibility = Visibility.Hidden;
            }
        }
    }
}
