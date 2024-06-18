using Clinic.Business.Base;
using Clinic.Data.Models;
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
using System.Net;
using Clinic.Business.Clinic;
using Clinic.WpfApp.UI.DetailWindow;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;

namespace Clinic.WpfApp.UI
{
    /// <summary>
    /// Interaction logic for wServices.xaml
    /// </summary>
    public partial class WServices : Window
    {
        private readonly ServiceBusiness _serviceBusiness;
        private readonly ClinicBusiness _clinicBusiness;

        public WServices()
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
                    var updatedService = item.Data as Service;

                    updatedService.ServiceId = serviceID;
                    updatedService.ClinicId = clinicID;
                    updatedService.Name = name;
                    updatedService.Price = price;
                    updatedService.Description = description;
                    updatedService.Warranty = warranty;
                    updatedService.Duration = duration;
                    updatedService.Type = type;
                    updatedService.Active = active;
                    updatedService.IsInsuranceAccepted = insurance;

                    var result = await _serviceBusiness.Update(updatedService);
                    System.Windows.MessageBox.Show(result.Message, "Save");
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
            try
            {
                ServiceId.Text = string.Empty;
                ClinicId.Text = string.Empty;
                Name.Text = string.Empty;
                Price.Text = string.Empty;
                Description.Text = string.Empty;
                Warranty.Text = string.Empty;
                Duration.Text = string.Empty;
                Type.Text = string.Empty;
                YesActive.IsChecked = null;
                YesInsurance.IsChecked = null;
                NoActive.IsChecked = null;
                NoInsurance.IsChecked = null;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error");
            }
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
    }
}
