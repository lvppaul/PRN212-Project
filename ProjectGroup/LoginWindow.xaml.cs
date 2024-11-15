using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private ApiEndpoint endpoint = new();
        public static string userId { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
        }
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var loginData = new
            {
                Email = username,
                Password = password
            };
            string json = JsonConvert.SerializeObject(loginData);
            var requestContent1 = new StringContent(json, Encoding.UTF8, "application/json");


            var requestContent = new MultipartFormDataContent();

            requestContent.Add(new StringContent(UsernameTextBox.Text), "Email");
            requestContent.Add(new StringContent(PasswordBox.Password), "Password");
            string apiUrl = endpoint.GetEndpoint();


            using (var client = new HttpClient())
            {   
                try
                {
                    var response = await client.PostAsync("https://localhost:7062/api/Account/SignIn", requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        if (content.Contains("userId"))
                        {
                            var jsonObj = JObject.Parse(content);
                            userId = jsonObj["userId"]?.ToString();
                            MainWindow mainWindow = new MainWindow(userId);
                            MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            mainWindow.Show();
                        }
                        else if (content.Contains("Invalid username or password"))
                        {
                            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        if (errorContent.Contains("Invalid username or password"))
                        {
                            MessageBox.Show("Invalid username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    if (!response.IsSuccessStatusCode)
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

        private void Window_DpiChanged(object sender, DpiChangedEventArgs e)
        {

        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                textEmail.Visibility = Visibility.Collapsed; 
            }
            else
            {
                textEmail.Visibility = Visibility.Visible; 
            }
          
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                textPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                textPassword.Visibility = Visibility.Visible;
            }
        }

        private void tbRegister_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
