using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private HubConnection _connection;

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(txtFirstName.Text)
                || String.IsNullOrEmpty(txtLastName.Text)
                || String.IsNullOrEmpty(txtEmail.Text)
                || String.IsNullOrEmpty(txtPassword.Password)
                || String.IsNullOrEmpty(txtConfirmPassword.Password)

                )
            {
                MessageBox.Show("All field is required", "Validation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Confirm password does not match the password.", "Validation", MessageBoxButton.OK, MessageBoxImage.Error);
                return; 
            }

            RegisterDtoRequest registerDtoRequest = new RegisterDtoRequest
            {
                FirstName = txtFirstName.Text,    
                LastName = txtLastName.Text,     
                Email = txtEmail.Text,            
                Password = txtPassword.Password,  
                ConfirmPassword = txtConfirmPassword.Password 
            };
            string json = JsonConvert.SerializeObject(registerDtoRequest);
            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync("https://localhost:7062/api/Account/CreateMemberAccount", requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Register Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        string content = await response.Content.ReadAsStringAsync();
                         await  InitializeSignalRConnection();
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error: {errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async Task InitializeSignalRConnection()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7062/notificationHub")
                .Build();

            _connection.On<string>("ReceiveMessage", (message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    txtMessage.Text = message;
                });
            });

            try
            {
                await _connection.StartAsync();
                MessageBox.Show("Please go to your email to confirm.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error establishing SignalR connection: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void showLog(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close(); 
        }

        private void Close(object sender, RoutedEventArgs e)
        {

        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.StopAsync();
            }
        }
    }
}
