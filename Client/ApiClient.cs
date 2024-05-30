using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;

using ClassLibrary;
using Newtonsoft.Json;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Serilog.Events;


namespace Client
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

                HttpResponseMessage response = await client.GetAsync($"{apiUrl}health");
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

        public async Task SendImageAndReceiveJSONAsync(BitmapImage bitmapImage, string apiUrl)
        {
            try
            {
                window.activyVideoPage.VideoImage.Source = bitmapImage;

                byte[] imageBytes = await ImageConverter.ConvertImageToByteArrayAsync(bitmapImage);

                MultipartFormDataContent form = new()
                {
                    { new ByteArrayContent(imageBytes), "image", "image.png" }
                };

                if (window.isServerAlive)
                {
                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}file/", form);
                }
                else
                {
                    Logger.LogByTemplate(LogEventLevel.Error, note: "Server is not alive and you try to recive response from him");
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
        public async Task<List<ObjectOnPhoto>> GetObjectsOnFrame(BitmapImage bitmapImage, string apiUrl)
        {
            try
            {
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

                HttpResponseMessage response = await client.PostAsync($"{apiUrl}file/", form);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ResponseObject responseObject = JsonConvert.DeserializeObject<ResponseObject>(responseContent);

                    List<ObjectOnPhoto> objectsOnPhoto = new List<ObjectOnPhoto>(responseObject.Objects);
                    return objectsOnPhoto;
                }
                else
                {
                    MessageBox.Show(response.StatusCode.ToString());
                    Logger.LogByTemplate(LogEventLevel.Warning, note: $"HTTP request failed with status code {response.StatusCode}.");
                    return null;
                }
            }
            catch (HttpRequestException httpEx)
            {
                Logger.LogByTemplate(LogEventLevel.Error, httpEx, note: "HTTP request error while sending image.");
            }
            catch (Exception ex)
            {
                Logger.LogByTemplate(LogEventLevel.Error, ex, note: "Error while sending image.");
            }
            return null;
        }

        public async Task<List<List<ObjectOnPhoto>>> GetObjectsOnFrames(VideoCapture videoCapture, string apiUrl)
        {
            List<List<ObjectOnPhoto>> result = [];
            Mat frame = new();

            videoCapture.Set(VideoCaptureProperties.PosFrames, 0);

            window.activyVideoPage.ProcessVideoProgressBar.Minimum = 0;
            window.activyVideoPage.ProcessVideoProgressBar.Maximum = videoCapture.FrameCount;
            window.activyVideoPage.ProcessVideoProgressBar.Visibility = Visibility.Visible;

            videoCapture.Set(VideoCaptureProperties.PosFrames, 0);

            for (int i = 0; i < videoCapture.FrameCount; i++)
            {
                if (await CheckHealthAsync(apiUrl) == false)
                {
                    MessageBox.Show("Failed Health Check", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
                videoCapture.Read(frame);
                List<ObjectOnPhoto> checkerObjects = await GetObjectsOnFrame(ImageConverter.ImageSourceForImageControl(frame.ToBitmap()), apiUrl);
                if (checkerObjects != null) result.Add(checkerObjects);
                window.activyVideoPage.ProcessVideoProgressBar.Value = i;
            }
            window.activyVideoPage.ProcessVideoProgressBar.Visibility = Visibility.Hidden;
            return result;
        }

        public async Task GetProcessedInRealTimeVideo(VideoCapture videoCapture, VideoController videoController, string apiUrl)
        {
            videoController.ObjectsOnFrame = new();
            Mat frame = new();

            videoCapture.Set(VideoCaptureProperties.PosFrames, 0);

            window.activyVideoPage.ProcessVideoProgressBar.Minimum = 0;
            window.activyVideoPage.ProcessVideoProgressBar.Maximum = videoCapture.FrameCount;
            window.activyVideoPage.ProcessVideoProgressBar.Visibility = Visibility.Visible;


            for (int i = 0; i < videoCapture.FrameCount; i++)
            {
                videoCapture.Read(frame);
                List<ObjectOnPhoto> checkerObjects = await MainWindow.apiClient.GetObjectsOnFrame(ImageConverter.ImageSourceForImageControl(frame.ToBitmap()), ConnectionWindow.ConnectionUri);
                if (checkerObjects != null) videoController.ObjectsOnFrame.Add(checkerObjects);
                window.activyVideoPage.ProcessVideoProgressBar.Value = i;
            }
            window.activyVideoPage.ProcessVideoProgressBar.Visibility = Visibility.Hidden;
        }

        //TODO. Only for lead. I'm sure you'll remember tomorrow what you want to do here. Doubtful solid. It is in the directory with the processor.
    }
}