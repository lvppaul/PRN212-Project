using Newtonsoft.Json;
using ProjectGroup.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace ProjectGroup
{
    /// <summary>
    /// Interaction logic for CreateKoiWindow.xaml
    /// </summary>
    public partial class CreateKoiWindow : Window
    {

        private string UserId { get; set; }
        public KoiDtosRequest CreatedKoi { get; set; }

        public CreateKoiWindow(string userId)
        {
            InitializeComponent();
            UserId = userId;
            txtUserId.Text = userId;
        }

        private async void btnSaveCreate_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text) ||
         String.IsNullOrEmpty(txtAge.Text) ||
         String.IsNullOrEmpty(txtColor.Text) ||
         String.IsNullOrEmpty(txtLength.Text) ||
         String.IsNullOrEmpty(txtNote.Text) ||
         String.IsNullOrEmpty(txtVariety.Text) ||
         String.IsNullOrEmpty(txtWeight.Text) ||
         String.IsNullOrEmpty(txtPhysique.Text) ||
         String.IsNullOrEmpty(txtOrigin.Text) ||
         cmbSex.SelectedItem == null ||
         cbxPond.SelectedValue == null)
            {
                MessageBox.Show("All fields are required", "Validation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (!int.TryParse(txtAge.Text, out int age))
            {
                MessageBox.Show("Please enter a valid age.");
                return;
            }

            if (!float.TryParse(txtWeight.Text, out float weight))
            {
                MessageBox.Show("Please enter a valid weight.");
                return;
            }

            if (!int.TryParse(txtLength.Text, out int length))
            {
                MessageBox.Show("Please enter a valid length.");
                return;
            }

            KoiDtosRequest koiDtosRequest = new KoiDtosRequest
            {
                UserId = txtUserId.Text,
                PondId = int.Parse(cbxPond.SelectedValue.ToString()),
                Name = txtName.Text,
                Variety = txtVariety.Text,
                Weight = weight,
                Length = length,
                Color = txtColor.Text,
                Age = age,
                Physique = txtPhysique.Text,
                Origin = txtOrigin.Text,
                Status = chkStatus.IsChecked.HasValue ? chkStatus.IsChecked.Value : false,
                Sex = (cmbSex.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Unknown",
                Note = txtNote.Text
            };
            string json = JsonConvert.SerializeObject(koiDtosRequest);
            var requestContent1 = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync("https://localhost:7062/api/koi", requestContent1);

                    if (response.IsSuccessStatusCode)
                    {
                        
                        MessageBox.Show("Create Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private  async void LoadPond()
        {

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7062/api/Pond/GetPondsByUserId/{UserId}");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var pondsList = JsonConvert.DeserializeObject<List<PondDtosResponse>>(content);
                        cbxPond.ItemsSource = pondsList;
                        cbxPond.DisplayMemberPath = "Name";
                        cbxPond.SelectedValuePath = "PondId";
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPond();
        }
    }
}
