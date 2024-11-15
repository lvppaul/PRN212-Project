using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectGroup.Dtos;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MaterialDesignThemes.Wpf.Theme;

namespace ProjectGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string userId { get; set; }
         
        public List<KoiDtosResponse> listKoiDtosResponses { get; set; }
        public MainWindow(string userId)
        {       
            InitializeComponent();
            MainWindow.userId = userId;
        }
        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            // Set tooltip visibility

            if (Tg_Btn.IsChecked == true)
            {
                tt_home.Visibility = Visibility.Collapsed;
                tt_contacts.Visibility = Visibility.Collapsed;
                tt_messages.Visibility = Visibility.Collapsed;
                tt_maps.Visibility = Visibility.Collapsed;
                tt_settings.Visibility = Visibility.Collapsed;
                tt_signout.Visibility = Visibility.Collapsed;
            }
            else
            {
                tt_home.Visibility = Visibility.Visible;
                tt_contacts.Visibility = Visibility.Visible;
                tt_messages.Visibility = Visibility.Visible;
                tt_maps.Visibility = Visibility.Visible;
                tt_settings.Visibility = Visibility.Visible;
                tt_signout.Visibility = Visibility.Visible;
            }
        }
        //Call api to Koi Reponse 
            public async Task LoadKoiAsync()
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        var response = await client.GetAsync($"https://localhost:7062/api/Koi/GetAllKoiByUserId?userid={userId}");

                        if (response.IsSuccessStatusCode)
                        {
                            string content = await response.Content.ReadAsStringAsync();
                            var koiList = JsonConvert.DeserializeObject<List<KoiDtosResponse>>(content);
                        listKoiDtosResponses = koiList;
                        dataGridKoi.ItemsSource = koiList;
                            dataGridKoi.Visibility = Visibility.Visible;
                            dataGridPond.Visibility = Visibility.Collapsed;
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

        public async Task LoadPondAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7062/api/Pond/GetPondsByUserId/{userId}");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var pondList = JsonConvert.DeserializeObject<List<PondDtosResponse>>(content);
                        dataGridPond.ItemsSource = pondList;
                        dataGridKoi.Visibility = Visibility.Visible;
                        dataGridPond.Visibility = Visibility.Collapsed;

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


        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            img_bg.Opacity = 1;
        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            img_bg.Opacity = 0.3;
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadKoiAsync();
            LoadPondAsync();
            LoadPond();
            dataGridKoi.Visibility = Visibility.Visible;
            dataGridPond.Visibility = Visibility.Collapsed;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                var koiId = button.Tag as int?;
                if (koiId.HasValue)
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            var response = await client.DeleteAsync($"https://localhost:7062/api/Koi/{koiId.Value}");

                            if (response.IsSuccessStatusCode)
                            {
                                MessageBox.Show($"Koi with ID {koiId.Value} deleted successfully.");

                                var koiList = dataGridKoi.ItemsSource as List<KoiDtosResponse>;
                                if (koiList != null)
                                {
                                    var koiToRemove = koiList.FirstOrDefault(k => k.KoiId == koiId.Value);
                                    if (koiToRemove != null)
                                    {
                                        koiList.Remove(koiToRemove);
                                        dataGridKoi.ItemsSource = null;
                                        dataGridKoi.ItemsSource = koiList; 
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete the Koi.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid KoiId.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            CreateKoiWindow createKoiWindow = new CreateKoiWindow(userId);
            bool? result = createKoiWindow.ShowDialog();

            if (result == true)
            {
                LoadKoiAsync();
            }
            else
            {
                MessageBox.Show("Action cancelled.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void btnUpdate_Click_1(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                var koiId = button.Tag as int?;
                if (koiId.HasValue)
                {

                    var koiList = dataGridKoi.ItemsSource as List<KoiDtosResponse>;
                    if (koiList != null)
                    {
                        var koiToUpdate = koiList.FirstOrDefault(k => k.KoiId == koiId.Value);
                        if (koiToUpdate != null)
                        {
                            var editWindow = new EditKoiWindow(koiToUpdate);
                            bool? result = editWindow.ShowDialog();
                            if (result == true)
                            {
                                var updatedKoi = editWindow.UpdatedKoi;

                                using (var client = new HttpClient())
                                {
                                    try
                                    {
                                        var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedKoi), Encoding.UTF8, "application/problem+json");
                                        var response = await client.PutAsync($"https://localhost:7062/api/Koi/{koiId.Value}", jsonContent);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            MessageBox.Show($"Koi with ID {koiId.Value} updated successfully.");

                                            koiToUpdate.Name = updatedKoi.Name;
                                            koiToUpdate.Sex = updatedKoi.Sex;
                                            koiToUpdate.Variety = updatedKoi.Variety;
                                            koiToUpdate.Physique = updatedKoi.Physique;
                                            koiToUpdate.Note = updatedKoi.Note;
                                            koiToUpdate.Origin = updatedKoi.Origin;
                                            koiToUpdate.Color = updatedKoi.Color;
                                            koiToUpdate.Age = updatedKoi.Age;
                                            koiToUpdate.Status = updatedKoi.Status;

                                            dataGridKoi.ItemsSource = null;
                                            dataGridKoi.ItemsSource = koiList;
                                            LoadPondAsync();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Failed to update the Koi.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    else
                    {
                        MessageBox.Show("Invalid KoiId.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnKoiView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dataGridKoi.Visibility = Visibility.Visible;
            dataGridPond.Visibility = Visibility.Collapsed;
        }

        private void btnPondView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dataGridKoi.Visibility = Visibility.Collapsed;
            dataGridPond.Visibility = Visibility.Visible; 

        }

        private async void btnDeletePond_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                var pondId = button.Tag as int?;
                if (pondId.HasValue)
                {
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            var response = await client.DeleteAsync($"https://localhost:7062/api/Pond/{pondId.Value}");

                            if (response.IsSuccessStatusCode)
                            {
                                MessageBox.Show($"Pond with ID {pondId.Value} deleted successfully.");

                                var pondsList = dataGridKoi.ItemsSource as List<PondDtosResponse>;
                                if (pondsList != null)
                                {
                                    var pondToRemove = pondsList.FirstOrDefault(k => k.PondId == pondId.Value);
                                    if (pondToRemove != null)
                                    {
                                        pondsList.Remove(pondToRemove);
                                        dataGridKoi.ItemsSource = null;
                                        dataGridKoi.ItemsSource = pondsList;
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete the Pond.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Invalid KoiId.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void btnUpdatePond_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button != null)
            {
                var pondId = button.Tag as int?;
                if (pondId.HasValue)
                {

                    var pondsList = dataGridPond.ItemsSource as List<PondDtosResponse>;
                    if (pondsList != null)
                    {
                        var pondToUpdate = pondsList.FirstOrDefault(k => k.PondId == pondId.Value);
                        if (pondToUpdate != null)
                        {
                            var editWindow = new EditPondWindow(pondToUpdate);
                            bool? result = editWindow.ShowDialog();
                            if (result == true)
                            {
                                var updatedPond = editWindow.UpdatedPond;

                                using (var client = new HttpClient())
                                {
                                    try
                                    {
                                        var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedPond), Encoding.UTF8, "application/problem+json");
                                        var response = await client.PutAsync($"https://localhost:7062/api/Pond/{pondId.Value}", jsonContent);

                                        if (response.IsSuccessStatusCode)
                                        {
                                            MessageBox.Show($"Koi with ID {pondId.Value} updated successfully.");

                                            pondToUpdate.Name = updatedPond.Name;
                                            pondToUpdate.Volume = updatedPond.Volume;
                                            pondToUpdate.Depth = updatedPond.Depth;
                                            pondToUpdate.PumpingCapacity = updatedPond.PumpingCapacity;
                                            pondToUpdate.Drain = updatedPond.Drain;
                                            pondToUpdate.Skimmer = updatedPond.Skimmer;
                                            pondToUpdate.Note = updatedPond.Note;

                                            // Cập nhật lại nguồn dữ liệu cho DataGrid
                                            dataGridPond.ItemsSource = null;
                                            dataGridPond.ItemsSource = pondsList;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Failed to update the Pond.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    else
                    {
                        MessageBox.Show("Invalid PondId.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnCreatePond_Click(object sender, RoutedEventArgs e)
        {
            CreatePondWindow createPondWindow = new CreatePondWindow(userId);
            bool? result = createPondWindow.ShowDialog();

            if (result == true)
            {
                LoadPondAsync();
            }
            else
            {
                MessageBox.Show("Action cancelled.", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void LoadPond()
        {

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7062/api/Pond/GetPondsByUserId/{userId}");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var pondsList = JsonConvert.DeserializeObject<List<PondDtosResponse>>(content);
                        cbxPondName.ItemsSource = pondsList;
                        cbxPondName.DisplayMemberPath = "Name";
                        cbxPondName.SelectedValuePath = "PondId";
                        cbxPondNameFood.ItemsSource = pondsList;
                        cbxPondNameFood.DisplayMemberPath = "Name";
                        cbxPondNameFood.SelectedValuePath = "PondId";
                        cbxPondNameWithKois.ItemsSource = pondsList;
                        cbxPondNameWithKois.DisplayMemberPath = "Name";
                        cbxPondNameWithKois.SelectedValuePath = "PondId";
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
        private void btnSaltCaculate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dataGridKoi.Visibility = Visibility.Collapsed;
            dataGridPond.Visibility = Visibility.Collapsed;
        }

        private async  void btnCaculateSalt_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(txtCurrent.Text) ||
                String.IsNullOrEmpty(txtDesired.Text) ||
                String.IsNullOrEmpty(txtWaterPercent.Text) ||
                cbxPondName.SelectedValue ==null
                )
            {
                MessageBox.Show("All field is required","Validation",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ckbDecrease == null && ckbIncrease == null) { 
                MessageBox.Show("Please choose a method increase or decrease","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DecisionDtoRequest decisionDtoRequest = new DecisionDtoRequest();
            if (int.TryParse(cbxPondName.SelectedValue.ToString(), out int pondId))
            {
                decisionDtoRequest.PondId = pondId;
            }
            else
            {
                MessageBox.Show("Invalid Pond selection", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (float.TryParse(txtDesired.Text, out float desiredConcentration))
            {
                decisionDtoRequest.DesiredConcentration = desiredConcentration;
            }
            else
            {
                MessageBox.Show("Please enter a valid Desired Concentration", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(txtCurrent.Text) && float.TryParse(txtCurrent.Text, out float currentConcentration))
            {
                decisionDtoRequest.CurrentConcentration = currentConcentration;
            }
            else
            {
                decisionDtoRequest.CurrentConcentration = null; 
            }

            if (int.TryParse(txtWaterPercent.Text, out int percentWaterChange))
            {
                decisionDtoRequest.PercentWaterChange = percentWaterChange;
            }
            else
            {
                MessageBox.Show("Please enter a valid percentage for water change", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string apiUrl = string.Empty;
            if (ckbIncrease.IsChecked == true && ckbDecrease.IsChecked == false)
            {
                apiUrl = "https://localhost:7062/api/SaltCalculate/increase-concentration";
            }
            else if (ckbDecrease.IsChecked == true && ckbIncrease.IsChecked == false)
            {
                apiUrl = "https://localhost:7062/api/SaltCalculate/decrease-concentration";
            }
            else
            {
                MessageBox.Show("Please select either Increase or Decrease method", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string json = JsonConvert.SerializeObject(decisionDtoRequest);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(apiUrl, requestContent);


                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<SaltCalculationResult>(content);
                        MessageBox.Show($"Amount of salt: {result.AmountOfSalt} \n" + 
                            $"Amount of Saft Refill: {result.AmountOfSaltRefill?.ToString() ?? " No need refill"} \n" +
                            $"Number of Changess: {result.NumberOfChanges?.ToString() ?? " No changes"} \n",
                            "Calculation Successful", MessageBoxButton.OK, MessageBoxImage.Information
                            );
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error: {errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        public async Task<double> CaculateFishWithWeight(int pondid)
        {

            double totalWeight = 0;
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7062/api/Pond/ListKoiInPond/{pondid}");


                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var koisList = JsonConvert.DeserializeObject<List<KoiDtosResponse>>(content);
                        if (koisList != null && koisList.Count > 0)
                        {
                            totalWeight = koisList.Sum(koi => koi.Weight/1000);  
                           return totalWeight;
                        }
                        else
                        {
                            MessageBox.Show("No koi found in the pond.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return 0;
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error: {errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return totalWeight;
        }

        public async Task<int> FindKoisInPond(int pondid)
        {
            int totalKois = 0;
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"https://localhost:7062/api/Pond/ListKoiInPond/{pondid}");


                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var koisList = JsonConvert.DeserializeObject<List<KoiDtosResponse>>(content);
                        if (koisList != null && koisList.Count > 0)
                        {
                           totalKois = koisList.Count;
                        }
                        else
                        {
                            MessageBox.Show("No koi found in the pond.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return 0;
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error: {errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return totalKois;
        }


        private void ckbIncrease_Checked(object sender, RoutedEventArgs e)
        {
            ckbDecrease.IsChecked = false;  

        }

        private void ckbDecrease_Checked(object sender, RoutedEventArgs e)
        {
            ckbIncrease.IsChecked = false;
        }

        private async void btnCaculateFood_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var totalWeight = await CaculateFishWithWeight(int.Parse(cbxPondNameFood.SelectedValue.ToString()));

                if (totalWeight <= 0)
                {
                    MessageBox.Show("No koi found in this pond", "Validate", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(txtFoodRate.Text) || !float.TryParse(txtFoodRate.Text, out float foodRate))
                {
                    MessageBox.Show("Please enter a valid food rate", "Validate", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var totalFood = Math.Round(totalWeight * foodRate, 2);

                MessageBox.Show($"The total amount of food required: {totalFood} kg",
                                "Calculation Result",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        private List<KoiDtosResponse> FilterKoi( List<KoiDtosResponse> koiList)
        {
            if (koiList == null) return new List<KoiDtosResponse>(); 

            HashSet<string> selectedNames = new HashSet<string>();

            if (ckbHotaru.IsChecked == true) selectedNames.Add("Hotaro");
            if (ckbNero.IsChecked == true) selectedNames.Add("Neru");
            if (ckbYama.IsChecked == true) selectedNames.Add("Yama");
            if (ckbEitoku.IsChecked == true) selectedNames.Add("Eitoku");
            if (ckbAnteiku.IsChecked == true) selectedNames.Add("Anteiku");
            Console.WriteLine("Selected Names: " + string.Join(", ", selectedNames));
            
            var filteredList = koiList.Where(k => selectedNames.Contains(k.Name)).ToList();

            if(selectedNames.Count == 0)
            {
                filteredList = koiList;
            }
            if(ckbAsc.IsChecked == true)
            {
                filteredList = filteredList.OrderBy(k => k.Age).ToList();
            }
            else if (ckbDesc.IsChecked == true)
            {
                filteredList = filteredList.OrderByDescending(k => k.Age).ToList();
            }

            if (ckbAscWeight.IsChecked == true) { 
                filteredList = filteredList.OrderBy(k => k.Weight).ToList();
            }
            else if (ckbDescWeight.IsChecked == true)
            {
                filteredList = filteredList.OrderByDescending(k => k.Weight).ToList();
            }
            return filteredList;

        }

        private void UpdateListView(List<KoiDtosResponse> filteredList)
        {
            dataGridKoi.ItemsSource = filteredList;
        }

        private void ckbHotaru_Checked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);

        }

        private void ckbNero_Checked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbYama_Checked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbEitoku_Checked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbAnteiku_Checked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbHotaru_UnChecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbNero_UnChecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbYama_UnChecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbEitoku_UnChecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbAnteiku_UnChecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbAsc_Checked(object sender, RoutedEventArgs e)
        {
            ckbDesc.IsChecked = false;
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbAsc_Unchecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbDesc_Checked(object sender, RoutedEventArgs e)
        {
            ckbAsc.IsChecked = false;
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbDesc_Unchecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbDescWeight_Checked(object sender, RoutedEventArgs e)
        {
            ckbAscWeight.IsChecked = false;
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbAscWeight_Unchecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbDescWeight_Unchecked(object sender, RoutedEventArgs e)
        {
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private void ckbAscWeight_Checked(object sender, RoutedEventArgs e)
        {
            ckbDescWeight.IsChecked = false;
            var filteredList = FilterKoi(listKoiDtosResponses);
            UpdateListView(filteredList);
        }

        private async void btnFindKois_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var totalKois = await FindKoisInPond(int.Parse(cbxPondNameWithKois.SelectedValue.ToString()));

                if (totalKois <= 0)
                {
                    MessageBox.Show("No koi found in this pond", "Empty pond", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"The total kois in this pond is: {totalKois}",
                                "Goodbye",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}