using Newtonsoft.Json.Linq;
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

namespace Project_KoiCareSystemAtHome
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private IHttpClientFactory _httpClientFactory;
        public static string userId { get; set; }  
        public Login( IHttpClientFactory httpClientFactory)
        {
            InitializeComponent();
            _httpClientFactory = httpClientFactory;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7062");
            HttpResponseMessage response = new HttpResponseMessage();

            var requestContent = new MultipartFormDataContent();

            requestContent.Add(new StringContent(txtEmail.Text), "Email");
            requestContent.Add(new StringContent(txtPass.Password), "Password");

            // Http Post 
            response = await client.PostAsync($"api/Account/SignIn", requestContent);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
               var jsonObj = JObject.Parse(result);
                userId = jsonObj["userId"]?.ToString();
                
                response.Dispose(); // don dep du lieu lay tu api
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                var result = response.Content.ReadAsStringAsync().Result;
                MessageBox.Show(result);

            }
           

        }
    }
}
