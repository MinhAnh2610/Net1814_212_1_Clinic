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
using System.Windows.Forms.Design;

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
                if (!int.TryParse(ClinicID.Text, out int clinicID) || !int.TryParse(ServiceID.Text, out int serviceID) || !decimal.TryParse(Price.Text, out decimal price))
                {
                    MessageBox.Show("Invalid type of input", "Warning");
                    return;
                }

                var existingClinic = await _clinicBusiness.GetById(clinicID);
                if (existingClinic.Data == null)
                {
                    MessageBox.Show("No clinic found", "Warning");
                    return;
                }

                String name = ServiceName.Text;
                String description = Description.Text;

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
                    };

                    var result = await _serviceBusiness.Save(service);
                    MessageBox.Show(result.Message, "Save");
                }
                else
                {
                    var updatedService = item.Data as Service;

                    updatedService.ServiceId = serviceID;
                    updatedService.ClinicId = clinicID;
                    updatedService.Name = name;
                    updatedService.Price = price;
                    updatedService.Description = description;

                    var result = await _serviceBusiness.Update(updatedService);
                    MessageBox.Show(result.Message, "Save");
                }

                ServiceID.Text = string.Empty;
                ClinicID.Text = string.Empty;
                ServiceName.Text = string.Empty;
                Price.Text = string.Empty;
                Description.Text = string.Empty;

                this.LoadGrdServices();
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

        private async void grdService_MouseDouble_Click(object sender, RoutedEventArgs e)
        {
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
                            ServiceID.Text = item.ServiceId.ToString();
                            ClinicID.Text = item.ClinicId.ToString();
                            ServiceName.Text = item.Name;
                            Price.Text = item.Price.ToString();
                            Description.Text = item.Description;
                        }
                    }
                }
            }
        }

        private async void grdService_ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int serviceID = (int)btn.CommandParameter;
                MessageBoxResult askDelete = MessageBox.Show("Do you want to delete this?", "Delete", MessageBoxButton.YesNo);
                if (askDelete == MessageBoxResult.Yes)
                {
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
