using Clinic.Business.Base;
using Clinic.Business.Clinic;
using Clinic.Data.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for WClinic.xaml
    /// </summary>
    public partial class WClinic : Window
    {
        private readonly IClinicBusiness _clinicBusiness;

        public WClinic()
        {
            InitializeComponent();
            _clinicBusiness = new ClinicBusiness();
            LoadClinics();
        }

        private async void LoadClinics()
        {
            try
            {
                var clinics = await _clinicBusiness.GetAll();
                if (clinics.Status > 0 && clinics.Data != null)
                {
                    clinicList.ItemsSource = clinics.Data as List<Data.Models.Clinic>;
                }
                else 
                { 
                    clinicList.ItemsSource = new List<Data.Models.Clinic>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var clinic = new Data.Models.Clinic()
                {
                    ClinicId = Int32.Parse(ClinicId.Text),
                    OwnerName = OwnerName.Text,
                    Name = Name.Text,
                    Address = Address.Text,
                    Contact = Contact.Text,
                };

                var existingClinic = await _clinicBusiness.GetById(clinic.ClinicId);
                if (existingClinic.Data != null)
                {
                    MessageBox.Show("Clinic ID already exist", "Warning");
                }
                else {
                    var result = await _clinicBusiness.Save(clinic);
                    MessageBox.Show(result.Message, "Save");

                    ClinicId.Text = string.Empty;
                    OwnerName.Text = string.Empty;
                    Name.Text = string.Empty;
                    Address.Text = string.Empty;
                    Contact.Text = string.Empty;

                    LoadClinics();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            ClinicId.Text = string.Empty;
            OwnerName.Text = string.Empty;
            Name.Text = string.Empty;
            Address.Text = string.Empty;
            Contact.Text = string.Empty;
        }

        private async void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button != null)
                {
                    int clinicId = (int)button.CommandParameter;

                    // Fetch the clinic details using the ClinicId
                    var existingClinic = await _clinicBusiness.GetById(clinicId);
                    var clinicModel = existingClinic.Data as Data.Models.Clinic;

                    // Update the form fields with the clinic details
                    if (clinicModel != null)
                    {
                        ClinicId.Text = clinicModel.ClinicId.ToString();
                        OwnerName.Text = clinicModel.OwnerName;
                        Name.Text = clinicModel.Name;
                        Address.Text = clinicModel.Address;
                        Contact.Text = clinicModel.Contact;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
        }

        private async void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var clinicUpdate = new Data.Models.Clinic()
                {
                    ClinicId = Int32.Parse(ClinicId.Text),
                    OwnerName = OwnerName.Text,
                    Name = Name.Text,
                    Address = Address.Text,
                    Contact = Contact.Text,
                };

                var existingClinic = await _clinicBusiness.GetById(clinicUpdate.ClinicId);
                var clinicModel = existingClinic.Data as Data.Models.Clinic;
                if (existingClinic.Data == null)
                {
                    MessageBox.Show("Clinic ID doesn't exist", "Warning");
                }
                else if (clinicModel != null)
                {
                    clinicModel.OwnerName = clinicUpdate.OwnerName;
                    clinicModel.Name = clinicUpdate.Name;
                    clinicModel.Address = clinicUpdate.Address;
                    clinicModel.Contact = clinicUpdate.Contact;

                    var result = await _clinicBusiness.Update(clinicModel);
                    MessageBox.Show(result.Message, "Update");

                    ClinicId.Text = string.Empty;
                    OwnerName.Text = string.Empty;
                    Name.Text = string.Empty;
                    Address.Text = string.Empty;
                    Contact.Text = string.Empty;

                    LoadClinics();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
        }

        private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button != null)
                {
                    int clinicId = (int)button.CommandParameter;

                    // Show confirmation message box
                    MessageBoxResult confirm = MessageBox.Show("Are you sure you want to delete this clinic?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (confirm == MessageBoxResult.Yes)
                    {
                        var result = await _clinicBusiness.DeleteById(clinicId);
                        MessageBox.Show(result.Message, "Delete");
                    }

                    // Update the form fields with the clinic details
                    LoadClinics();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", "Error");
            }
        }
    }
}
