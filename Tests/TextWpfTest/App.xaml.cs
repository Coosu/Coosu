using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coosu.Storyboard.Advanced.Text;
using Size = System.Windows.Size;

namespace TextWpfTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Image FromControlToImage(Size size, FrameworkElement fe)
        {
            fe.Measure(size);
            fe.Arrange(new Rect(size));
            fe.UpdateLayout();
            var dpi = 96;
            var dpiVector2 = new Vector2(dpi, dpi);

            var bitmap = new RenderTargetBitmap(
                (int)(size.Width * dpiVector2.X / 96), (int)(size.Height * dpiVector2.Y / 96),
                dpiVector2.X, dpiVector2.Y, PixelFormats.Pbgra32
            );

            bitmap.Render(fe);
            using var stream = new MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);
            var bitmap1 = new Bitmap(stream);
            return bitmap1;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var control = new TextControl();
            var image = FromControlToImage(new Size(800, 450), control);
            image.Save("haha.png");
        }
    }
}
