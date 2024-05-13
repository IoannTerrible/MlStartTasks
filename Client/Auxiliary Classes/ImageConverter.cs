using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

public class ImageConverter
{
    public static BitmapImage ImageSourceForImageControl(System.Drawing.Bitmap bitmap)
    {
        using MemoryStream memory = new();
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        memory.Position = 0;
        BitmapImage bitmapimage = new();
        bitmapimage.BeginInit();
        bitmapimage.StreamSource = memory;
        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapimage.EndInit();
        return bitmapimage;
    }
    public static async Task<byte[]> ConvertImageToByteArrayAsync(BitmapImage bitmapImage)
    {
        using (MemoryStream stream = new MemoryStream())
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(stream);
            return stream.ToArray();
        }
    }
}
