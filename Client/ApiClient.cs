using ClassLibrary;
using Newtonsoft.Json;
using Serilog.Events;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SocketClient
{
    public class ApiClient
    {
        private readonly HttpClient client;
        private MainWindow window;
        public ApiClient(MainWindow window)
        {
            client = new HttpClient();
            this.window = window;
        }

        public async Task<bool> CheckHealthAsync(string apiUrl)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    string jsonObject = JsonConvert.DeserializeObject<string>(responseContent);
                    Dictionary<string, string> dictionaryResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonObject);
                    if (dictionaryResponse.ContainsKey("Status") && dictionaryResponse["Status"] == "OK")
                    {
                        return true;
                    }
                    else
                    {
                        Logger.LogByTemplate(LogEventLevel.Warning, note: "Health check response does not contain expected status.");
                    }
                }
                else
                {
                    Logger.LogByTemplate(LogEventLevel.Warning, note: $"HTTP request failed with status code {response.StatusCode}.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogByTemplate(LogEventLevel.Error, httpEx, note: "HTTP request error while performing health check.");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error while performing health check.");
            }
            return false;
        }


        public async Task SendImageAndReceiveJSONAsync(string imageUrl, string apiUrl)
        {
            try
            {
                Uri trueImageUrl = new Uri(imageUrl);
                window.activyImagePage.ImageBoxThree.Source = new BitmapImage(trueImageUrl);

                byte[] imageBytes = System.IO.File.ReadAllBytes(imageUrl);
                MultipartFormDataContent form = new()
                {
                    { new ByteArrayContent(imageBytes), "image", "image.jpg" }
                };

                if (await CheckHealthAsync($"{apiUrl}health"))
                {
                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}file", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        ResponseObject responseObject = JsonConvert.DeserializeObject<ResponseObject>(responseContent);

                        List<ObjectOnPhoto> objectsOnPhoto = new List<ObjectOnPhoto>(responseObject.Objects);
                        window.activyImagePage.DrawBoundingBoxes(objectsOnPhoto);
                        string[] parts = responseContent.Split(",");

                        parts[0] = parts[0].Substring(1);
                        int lastIndex = parts.Length - 1;
                        parts[lastIndex] = parts[lastIndex].Substring(0, parts[lastIndex].Length - 1);
                        StringBuilder tempStringBuilder = new StringBuilder();
                        foreach (string par in parts)
                        {
                            tempStringBuilder.Append(par);
                            tempStringBuilder.Append(" "); 
                            window.activyImagePage.ListBoxForResponce.Items.Add(par);
                        }

                        string tempString = tempStringBuilder.ToString();
                        Logger.LogByTemplate(LogEventLevel.Information, note: $"Response for server {tempString}");
                    }
                    else
                    {
                        MessageBox.Show(response.StatusCode.ToString());
                        Logger.LogByTemplate(LogEventLevel.Warning, note: $"HTTP request failed with status code {response.StatusCode}.");
                    }
                }
                else
                {
                    MessageBox.Show("Health check failed before sending image");

                    Logger.LogByTemplate(LogEventLevel.Warning, note: "Health check failed before sending image.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"HTTP request error: {httpEx.Message}");
                Logger.LogByTemplate(LogEventLevel.Error, httpEx, note: "HTTP request error while sending image.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending image: {ex.Message}");
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error while sending image.");
            }
        }
        
        public async Task SendImageAndReceiveJSONAsync(BitmapImage bitmapImage, string apiUrl)
        {
            try
            {
                window.activyVideoPage.VideoImage.Source = bitmapImage;

                byte[] imageBytes;
                using (MemoryStream stream = new MemoryStream())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(stream);
                    imageBytes = stream.ToArray();
                }

                MultipartFormDataContent form = new()
                {
                    { new ByteArrayContent(imageBytes), "image", "image.png" }
                };

                //if (await CheckHealthAsync($"{apiUrl}health"))
                //{
                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}file/", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        ResponseObject responseObject = JsonConvert.DeserializeObject<ResponseObject>(responseContent);

                        List<ObjectOnPhoto> objectsOnPhoto = new List<ObjectOnPhoto>(responseObject.Objects);
                        window.activyVideoPage.localDrawer.DrawBoundingBoxes(objectsOnPhoto);
                        string[] parts = responseContent.Split(",");

                        parts[0] = parts[0].Substring(1);
                        int lastIndex = parts.Length - 1;
                        parts[lastIndex] = parts[lastIndex].Substring(0, parts[lastIndex].Length - 1);
                        StringBuilder tempStringBuilder = new StringBuilder();
                        string tempString = tempStringBuilder.ToString();
                        Logger.LogByTemplate(LogEventLevel.Information, note: $"Response for server {tempString}");
                    }
                    else
                    {
                        MessageBox.Show(response.StatusCode.ToString());
                        Logger.LogByTemplate(LogEventLevel.Warning, note: $"HTTP request failed with status code {response.StatusCode}.");
                    }
                //}
                //else
                //{
                //    MessageBox.Show("Health check failed before sending image");
                //    Logger.LogByTemplate(LogEventLevel.Warning, note: "Health check failed before sending image.");
                //}
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"HTTP request error: {httpEx.Message}");
                Logger.LogByTemplate(LogEventLevel.Error, httpEx, note: "HTTP request error while sending image.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending image: {ex.Message}");
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error while sending image.");
            }
        }
    }
}