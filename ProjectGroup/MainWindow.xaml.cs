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

namespace ProjectGroup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string userId { get; set; }

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
            dataGridKoi.Visibility = Visibility.Visible;
            dataGridPond.Visibility = Visibility.Collapsed;
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
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
            var button = sender as Button;
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
            var button = sender as Button;
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
            var button = sender as Button;
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
    }
}