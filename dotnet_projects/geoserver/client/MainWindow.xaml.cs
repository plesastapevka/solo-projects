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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient client;
        private const string URL = "http://localhost:8080/api/geoserver?";
        public MainWindow()
        {
            InitializeComponent();
            responseLabel.Content = "";
            pixelHeightLabel.Content = "";
            pixelWidthLabel.Content = "";
            topLeftXLabel.Content = "";
            topLeftYLabel.Content = "";
            client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void sendRequest_Click(object sender, RoutedEventArgs e)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["bbox"] = bboxBox.Text.ToString();
            query["styles"] = "";
            query["request"] = requestBox.Text.ToString();
            query["layers"] = layersBox.Text.ToString();
            query["width"] = widthBox.Text.ToString();
            query["height"] = heightBox.Text.ToString();
            query["srs"] = "XXX";
            query["format"] = formatBox.Text.ToString();
            string queryString = query.ToString();
            HttpResponseMessage response = client.GetAsync("?" + queryString).Result;
            if (response.IsSuccessStatusCode)
            {
                switch (requestBox.Text.ToString())
                {
                    case "GetMap":
                        Task<byte[]> buffer = response.Content.ReadAsByteArrayAsync();
                        MemoryStream memStream = new MemoryStream(buffer.Result);
                        BitmapImage img = new BitmapImage();
                        if(buffer.Result.Length != 0)
                        {
                            img.BeginInit();
                            img.StreamSource = memStream;
                            img.EndInit();
                            image.Width = img.Width;
                            image.Height = img.Height;
                            image.Source = img;
                            responseLabel.Foreground = Brushes.DarkGreen;
                            responseLabel.Content = response.StatusCode;
                        } 
                        else
                        {
                            responseLabel.Foreground = Brushes.Red;
                            responseLabel.Content = "Error: Received empty stream";
                        }
                        break;
                    case "GetCapabilities":
                        TfwParams parameters = JsonConvert.DeserializeObject<TfwParams>(response.Content.ReadAsStringAsync().Result);
                        pixelHeightLabel.Content = "Pixel height: " + parameters.height.ToString();
                        pixelWidthLabel.Content = "Pixel width: " + parameters.width.ToString();
                        topLeftXLabel.Content = "Top left X: " + parameters.pos_x.ToString();
                        topLeftYLabel.Content = "Top left Y: " + parameters.pos_y.ToString();
                        responseLabel.Foreground = Brushes.DarkGreen;
                        responseLabel.Content = response.StatusCode;
                        break;
                }
                Console.WriteLine("Success");
            }
            else
            {
                responseLabel.Foreground = Brushes.Red;
                responseLabel.Content = "Error: " + response.StatusCode;
                Console.WriteLine("Fail");
            }
        }

        private void widthBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void heightBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void layersBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void formatBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void requestBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void bboxBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
