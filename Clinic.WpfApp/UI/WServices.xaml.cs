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
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(ClinicID.Text, out int clinicID) || !int.TryParse(ServiceID.Text, out int serviceID) || !decimal.TryParse(Price.Text, out decimal price))
                {
                    MessageBox.Show("Invalid type of input", "Warning");
                }
                else
                {
                    var existingClinic = await _clinicBusiness.GetById(clinicID);
                    if (existingClinic.Data == null)
                    {
                        MessageBox.Show("No clinic found", "Warning");
                        return;
                    }

                    var service = new Service()
                    {
                        ServiceId = serviceID,
                        ClinicId = clinicID,
                        Price = price,
                        Name = ServiceName.Text,
                        Description = Description.Text,
                    };

                    var item = await _serviceBusiness.GetById(service.ServiceId);
                    if (item.Data == null)
                    {

                        var result = await _serviceBusiness.Save(service);
                        MessageBox.Show(result.Message, "Save");

                        this.LoadGrdServices();

                        ServiceID.Text = string.Empty;
                        ClinicID.Text = string.Empty;
                        ServiceName.Text = string.Empty;
                        Price.Text = string.Empty;
                        Description.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Exist service data", "Warning");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }

        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceID.Text = string.Empty;
                ClinicID.Text = string.Empty;
                ServiceName.Text = string.Empty;
                Price.Text = string.Empty;
                Description.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button != null)
                {
                    int serviceID = (int)button.CommandParameter;

                    var service = await _serviceBusiness.GetById(serviceID);
                    var serviceModel = service.Data as Data.Models.Service;

                    if (serviceModel != null)
                    {
                        ServiceID.Text = serviceModel.ServiceId.ToString();
                        ClinicID.Text = serviceModel.ClinicId.ToString();
                        ServiceName.Text = serviceModel.Name;
                        Price.Text = serviceModel.Price.ToString();
                        Description.Text = serviceModel.Description;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(ClinicID.Text, out int clinicID) || !int.TryParse(ServiceID.Text, out int serviceID) || !decimal.TryParse(Price.Text, out decimal price))
                {
                    MessageBox.Show("Invalid type of input", "Warning");
                    return;
                }

                var updatedService = new Service()
                {
                    ServiceId = serviceID,
                    ClinicId = clinicID,
                    Price = price,
                    Name = ServiceName.Text,
                    Description = Description.Text,
                };

                var existingService = await _serviceBusiness.GetById(updatedService.ServiceId);
                var service = existingService.Data as Service;

                if (existingService.Data == null)
                {
                    MessageBox.Show("Service doesn't exist", "Warning");
                    return;
                }

                if (service != null)
                {
                    service.ServiceId = updatedService.ServiceId;
                    service.ClinicId = updatedService.ClinicId;
                    service.Name = updatedService.Name;
                    service.Price = updatedService.Price;
                    service.Description = updatedService.Description;

                    var result = await _serviceBusiness.Update(service);
                    MessageBox.Show(result.Message, "Udpate");

                    this.LoadGrdServices();

                    ServiceID.Text = string.Empty;
                    ClinicID.Text = string.Empty;
                    ServiceName.Text = string.Empty;
                    Price.Text = string.Empty;
                    Description.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button != null)
                {
                    int serviceID = (int)button.CommandParameter;
                    var result = await _serviceBusiness.DeleteById(serviceID);
                    MessageBox.Show(result.Message, "Delete");

                    this.LoadGrdServices();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private async void LoadGrdServices ()
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
    }
}
