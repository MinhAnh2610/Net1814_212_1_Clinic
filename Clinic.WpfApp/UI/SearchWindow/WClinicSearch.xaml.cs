using Clinic.Business.Clinic;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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

namespace Clinic.WpfApp.UI.SearchWindow
{
    /// <summary>
    /// Interaction logic for WClinicSearch.xaml
    /// </summary>
    public partial class WClinicSearch : Window
    {
        private readonly IClinicBusiness _clinicBusiness;
        public WClinicSearch()
        {
            InitializeComponent();
            _clinicBusiness = new ClinicBusiness();
            LoadClinics(null);
        }

        private async void LoadClinics(List<Data.Models.Clinic> clinics)
        {
            try
            {
                if (clinics != null)
                {
                    clinicList.ItemsSource = clinics;
                }
                else
                {
                    var clinicListGet = await _clinicBusiness.GetAll();
                    clinicList.ItemsSource = clinicListGet.Data as List<Data.Models.Clinic>;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }
        private void ButtonAdd_Click(Object sender, RoutedEventArgs e)
        { 
            this.Close();
        }
        private void ButtonCancel_Click(Object sender, RoutedEventArgs e)
        {
            ClinicId.Text = string.Empty;
            OwnerName.Text = string.Empty;
            Name.Text = string.Empty;
            Address.Text = string.Empty;
            Contact.Text = string.Empty;
            Email.Text = string.Empty;
            Website.Text = string.Empty;
            ClinicType.Text = string.Empty;
            YesButton.IsChecked = false;
            NoButton.IsChecked = false;
            City.Text = string.Empty;
            Country.Text = string.Empty;
        }
        private async void ButtonSearch_Click(Object sender, RoutedEventArgs e)
        {
            var clinicGetAll = await _clinicBusiness.GetAll();
            var clinicList = clinicGetAll.Data as List<Data.Models.Clinic>;

            string clinicId = ClinicId.Text;
            string ownerName = OwnerName.Text;
            string name = Name.Text;
            string address = Address.Text;
            string contact = Contact.Text;
            string email = Email.Text;
            string website = Website.Text;
            string clinicType = ClinicType.Text;
            string isActive = ((bool)YesButton.IsChecked) ? "True" : "False";
            if (!(bool)YesButton.IsChecked && !(bool)NoButton.IsChecked)
            {
                isActive = null;   
            }
            string city = City.Text;
            string country = Country.Text;

            // Apply filters
            var filteredClinics = clinicList.Where(c =>
                (string.IsNullOrEmpty(clinicId) || c.ClinicId.ToString().Contains(clinicId)) &&
                (string.IsNullOrEmpty(ownerName) || c.OwnerName.Contains(ownerName, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(name) || c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(address) || c.Address.Contains(address, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(contact) || c.Contact.Contains(contact, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(email) || c.Email.Contains(email, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(website) || c.Website.Contains(website, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(clinicType) || c.ClinicType.Contains(clinicType, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(isActive) || c.IsActive == Boolean.Parse(isActive)) &&
                (string.IsNullOrEmpty(city) || c.City.Contains(city, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(country) || c.Country.Contains(country, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            LoadClinics(filteredClinics);
        }
    }
}
