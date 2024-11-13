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

        public MainWindow()
        {
            InitializeComponent();
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
                    var response = await client.GetAsync("https://localhost:7062/api/Koi");

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        var koiList = JsonConvert.DeserializeObject<List<KoiDtosResponse>>(content);
                       dataGridKoi.ItemsSource = koiList;
                       
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
                                        var jsonContent = new StringContent(JsonConvert.SerializeObject(updatedKoi), Encoding.UTF8, "application/json");
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
                                            koiToUpdate.Length = updatedKoi.Length;
                                            koiToUpdate.Weight = updatedKoi.Weight;
                                            koiToUpdate.Color = updatedKoi.Color;
                                            koiToUpdate.Status = updatedKoi.Status;

                                            dataGridKoi.ItemsSource = null;
                                            dataGridKoi.ItemsSource = koiList;
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
    }
}