using Clinic.Business.Clinic;
using Clinic.Data.Models;
using Clinic.WpfApp.UI.DetailWindow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml.Linq;
using static Azure.Core.HttpHeader;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Button = System.Windows.Controls.Button;

namespace Clinic.WpfApp.UI.SearchWindow
{
    /// <summary>
    /// Interaction logic for WServiceSearch.xaml
    /// </summary>
    public partial class WServiceSearch : Window
    {
        private readonly ServiceBusiness _serviceBusiness;
        private readonly ClinicBusiness _clinicBusiness;
        public WServiceSearch()
        {
            InitializeComponent();
            this._serviceBusiness = new ServiceBusiness();
            this._clinicBusiness = new ClinicBusiness();
            this.LoadGrdServices();
        }
        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(ClinicId.Text, out int clinicID) || !int.TryParse(ServiceId.Text, out int serviceID) || !decimal.TryParse(Price.Text, out decimal price))
                {
                    System.Windows.MessageBox.Show("Invalid type of input", "Warning");
                    return;
                }

                var existingClinic = await _clinicBusiness.GetById(clinicID);
                if (existingClinic.Data == null)
                {
                    System.Windows.MessageBox.Show("No clinic found", "Warning");
                    return;
                }

                String name = Name.Text;
                String description = Description.Text;
                String warranty = Warranty.Text;
                String duration = Duration.Text;
                String type = Type.Text;
                bool active = YesActive.IsChecked == true;
                bool insurance = YesInsurance.IsChecked == true;

                var item = await _serviceBusiness.GetById(serviceID);

                if (item.Data == null)
                {
                    var service = new Service()
                    {
                        ServiceId = serviceID,
                        ClinicId = clinicID,
                        Price = price,
                        Name = name,
                        Description = description,
                        Warranty = warranty,
                        Duration = duration,
                        Type = type,
                        Active = active,
                        IsInsuranceAccepted = insurance,
                    };

                    var result = await _serviceBusiness.Save(service);
                    System.Windows.MessageBox.Show(result.Message, "Save");
                }
                else
                {
                    System.Windows.MessageBox.Show("Service already exists.", "Warning");
                }

                this.ButtonCancel_Click(sender, e);

                this.LoadGrdServices();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error");
            }

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void grdService_ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int serviceID = (int)btn.CommandParameter;

                MessageBoxResult askDelete = System.Windows.MessageBox.Show("Do you want to delete this?", "Delete", MessageBoxButton.YesNo);
                if (askDelete == MessageBoxResult.Yes)
                {
                    var result = await _serviceBusiness.DeleteById(serviceID);
                    System.Windows.MessageBox.Show(result.Message, "Delete");
                    this.LoadGrdServices();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async void grdService_MouseDouble_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Double Click on Grid");
            DataGrid grd = sender as DataGrid;
            if (grd != null && grd.SelectedItems != null && grd.SelectedItems.Count == 1)
            {
                var row = grd.ItemContainerGenerator.ContainerFromItem(grd.SelectedItem) as DataGridRow;
                if (row != null)
                {
                    var item = row.Item as Service;
                    if (item != null)
                    {
                        var serviceResult = await _serviceBusiness.GetById(item.ServiceId);

                        if (serviceResult.Status > 0 && serviceResult.Data != null)
                        {
                            item = serviceResult.Data as Service;
                            ServiceId.Text = item.ServiceId.ToString();
                            ClinicId.Text = item.ClinicId.ToString();
                            Name.Text = item.Name;
                            Price.Text = item.Price.ToString();
                            Description.Text = item.Description;
                            Warranty.Text = item.Warranty;
                            Duration.Text = item.Duration;
                            Type.Text = item.Type;
                            YesActive.IsChecked = (item.Active ?? true) ? true : false;
                            YesInsurance.IsChecked = (item.IsInsuranceAccepted ?? true) ? true : false;
                        }
                    }
                }
            }
        }

        private async void grdService_ButtonView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int serviceID = (int)btn.CommandParameter;
                var result = await _serviceBusiness.GetById(serviceID);
                if (result.Data != null)
                {
                    var service = result.Data as Service;
                    var p = new WServiceDetail(service);
                    p.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error");
            }
        }
        private async void LoadGrdServices()
        {
            var result = await _serviceBusiness.GetAll();
            if (result.Status > 0 && result.Data != null)
            {
                grdService.ItemsSource = result.Data as List<Service>;
            }
            else
            {
                grdService.ItemsSource = new List<Service>();
            }
        }

        private void LoadGrdServices(List<Service> services)
        {
            if (services != null)
            {
                grdService.ItemsSource = services;
            }
            else
            {
                LoadGrdServices();
            }
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ServiceId.Text))
            {
                tbPlaceholder1.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder1.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(ClinicId.Text))
            {
                tbPlaceholder2.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder2.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Name.Text))
            {
                tbPlaceholder3.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder3.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Price.Text))
            {
                tbPlaceholder4.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder4.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Description.Text))
            {
                tbPlaceholder5.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder5.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Description.Text))
            {
                tbPlaceholder6.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder6.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Warranty.Text))
            {
                tbPlaceholder7.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder7.Visibility = Visibility.Hidden;
            }

            if (string.IsNullOrEmpty(Duration.Text))
            {
                tbPlaceholder8.Visibility = Visibility.Visible;
            }
            else
            {
                tbPlaceholder8.Visibility = Visibility.Hidden;
            }
        }

        private async void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            var result = await _serviceBusiness.GetAll();
            var services = result.Data as List<Service>;

            String serviceID = ServiceId.Text;
            String clinicID = ClinicId.Text;
            String price = Price.Text;
            String name = Name.Text;
            String description = Description.Text;
            String warranty = Warranty.Text;
            String duration = Duration.Text;
            String type = Type.Text;

            List<Service> sortedServices = services.Where(o =>
                (string.IsNullOrEmpty(serviceID) || o.ServiceId.ToString().Contains(serviceID)) &&
                (string.IsNullOrEmpty(clinicID) || o.ClinicId.ToString().Contains(clinicID)) &&
                (string.IsNullOrEmpty(price) || o.Price.ToString().Contains(price)) &&
                (string.IsNullOrEmpty(name) || o.Name.Contains(name)) &&
                (string.IsNullOrEmpty(description) || o.Description.Contains(description)) &&
                (string.IsNullOrEmpty(warranty) || o.Warranty.Contains(warranty)) &&
                (string.IsNullOrEmpty(duration) || o.Duration.Contains(duration)) &&
                (string.IsNullOrEmpty(type) || o.Type.Contains(type))
            ).ToList();

            this.LoadGrdServices(sortedServices);
        }
    }
}
