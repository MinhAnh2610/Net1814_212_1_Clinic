using Clinic.Business.Base;
using Clinic.Business.Clinic;
using Clinic.Data.Models;
using Clinic.WpfApp.UI.DetailWindow;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

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
                var existingClinic = await _clinicBusiness.GetById(Int32.Parse(ClinicId.Text));
                if (existingClinic.Data != null)
                {
                    var clinicModel = existingClinic.Data as Data.Models.Clinic;
                    var clinicUpdate = new Data.Models.Clinic()
                    {
                        ClinicId = Int32.Parse(ClinicId.Text),
                        OwnerName = OwnerName.Text,
                        Name = Name.Text,
                        Address = Address.Text,
                        Contact = Contact.Text,
                        Email = Email.Text,
                        Website = Website.Text,
                        ClinicType = ClinicType.Text,
                        IsActive = Boolean.Parse(IsActive.Text)
                    };

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
                    Email.Text = string.Empty;
                    Website.Text = string.Empty;
                    ClinicType.Text = string.Empty;
                    IsActive.Text = string.Empty;

                    LoadClinics();
                }
                else {
                    var clinic = new Data.Models.Clinic()
                    {
                        ClinicId = Int32.Parse(ClinicId.Text),
                        OwnerName = OwnerName.Text,
                        Name = Name.Text,
                        Address = Address.Text,
                        Contact = Contact.Text,
                        Email = Email.Text,
                        Website = Website.Text,
                        ClinicType = ClinicType.Text,
                        IsActive = Boolean.Parse(IsActive.Text)
                    };
                    var result = await _clinicBusiness.Save(clinic);
                    MessageBox.Show(result.Message, "Save");

                    ClinicId.Text = string.Empty;
                    OwnerName.Text = string.Empty;
                    Name.Text = string.Empty;
                    Address.Text = string.Empty;
                    Contact.Text = string.Empty;
                    Email.Text = string.Empty;
                    Website.Text = string.Empty;
                    ClinicType.Text = string.Empty;
                    IsActive.Text = string.Empty;

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
            Email.Text = string.Empty;
            Website.Text = string.Empty;
            ClinicType.Text = string.Empty;
            IsActive.Text = string.Empty;
        }

        private void ButtonGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var selectedClinic = button?.DataContext as Data.Models.Clinic;

                if (selectedClinic != null)
                {
                    var clinicWindow = new WClinicWindow(selectedClinic);
                    clinicWindow.Show();
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

        private async void Clinic_MouseDouble_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Double Click on Grid");
            DataGrid clinic = sender as DataGrid;
            if (clinic != null && clinic.SelectedItems != null && clinic.SelectedItems.Count == 1)
            {
                var row = clinic.ItemContainerGenerator.ContainerFromItem(clinic.SelectedItem) as DataGridRow;
                if (row != null)
                {
                    var item = row.Item as Data.Models.Clinic;
                    if (item != null)
                    {
                        var clinicResult = await _clinicBusiness.GetById(item.ClinicId);

                        if (clinicResult.Status > 0 && clinicResult.Data != null)
                        {
                            item = clinicResult.Data as Data.Models.Clinic;
                            ClinicId.Text = item.ClinicId.ToString();
                            OwnerName.Text = item.OwnerName;
                            Name.Text = item.Name;
                            Address.Text = item.Address;
                            Contact.Text = item.Contact;
                            Email.Text = item.Email;
                            Website.Text = item.Website;
                            ClinicType.Text = item.ClinicType;
                            IsActive.Text = item.IsActive.ToString();
                        }
                    }
                }
            }
        }
    }
}
