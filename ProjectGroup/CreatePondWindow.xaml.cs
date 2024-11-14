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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ProjectGroup
{
    /// <summary>
    /// Interaction logic for CreatePondWindow.xaml
    /// </summary>
    public partial class CreatePondWindow : Window
    {
        private string UserId { get; set; }
        public KoiDtosRequest CreatedPond { get; set; }

        public CreatePondWindow(string userId)
        {
            InitializeComponent();
            UserId = userId;
            txtUserId.Text = userId;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtPondName.Text) ||
       String.IsNullOrEmpty(txtDepth.Text) ||
       String.IsNullOrEmpty(txtDrain.Text) ||
       String.IsNullOrEmpty(txtSkimmer.Text) ||
       String.IsNullOrEmpty(txtNote.Text) ||
       String.IsNullOrEmpty(txtVolume.Text) ||
       String.IsNullOrEmpty(txtPumpingCapacity.Text))
            {
                MessageBox.Show("All fields are required", "Validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }

            if (!int.TryParse(txtVolume.Text, out int volume))
            {
                MessageBox.Show("Please enter a valid volume.");
                return;
            }

            if (!int.TryParse(txtPumpingCapacity.Text, out int pumpingCapacity))
            {
                MessageBox.Show("Please enter a valid Pumping Capacity.");
                return;
            }

            if (!int.TryParse(txtDrain.Text, out int drain))
            {
                MessageBox.Show("Please enter a valid drain.");
                return;
            }

            if (!int.TryParse(txtSkimmer.Text, out int skimmer))
            {
                MessageBox.Show("Please enter a valid skimmer.");
                return;
            }

            if (!float.TryParse(txtDepth.Text, out float depth))
            {
                MessageBox.Show("Please enter a valid depth.");
                return;
            }
            PondDtosRequest pondDtosRequest = new PondDtosRequest
            {
                userId = txtUserId.Text,  
                Name = txtPondName.Text,  
                Volume = volume,         
                Depth = (int)depth,       
                PumpingCapacity = pumpingCapacity,
                Drain = drain,
                Skimmer = skimmer,
                Note = txtNote.Text
            };
            string json = JsonConvert.SerializeObject(pondDtosRequest);
            var requestContent1 = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync("https://localhost:7062/api/Pond", requestContent1);

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
    }
}
