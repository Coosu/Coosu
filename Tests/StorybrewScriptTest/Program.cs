using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Coosu.Storyboard;
using Coosu.Storyboard.Advanced.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Formatting = Newtonsoft.Json.Formatting;
using Size = System.Windows.Size;

namespace StorybrewScriptTest
{
    /// <summary>
    ///     Stores a brush as XAML because Json.net has trouble saving it as JSON
    /// </summary>
    public class BrushJsonConverter : JsonConverter
    {
        //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //{
        //    var jo = new JObject { { "value", XamlWriter.Save(value) } };
        //    jo.WriteTo(writer);
        //}

        //public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        //    JsonSerializer serializer)
        //{
        //    if (reader.TokenType == JsonToken.Null) return null;
        //    // Load JObject from stream
        //    var jObject = JObject.Load(reader);
        //    return XamlReader.Parse(jObject["value"].ToString());
        //}

        //public override bool CanConvert(Type objectType)
        //{
        //    return typeof(Brush).IsAssignableFrom(objectType);
        //}

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Turn the brush into an XML node
            var doc = new XmlDocument();
            doc.LoadXml(XamlWriter.Save(value));

            // Serialize the XML node as JSON
            var jo = JObject.Parse(JsonConvert.SerializeXmlNode(doc.DocumentElement));
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            // Load JObject from stream
            var jObject = JObject.Load(reader);

            // Seriaze the JSON node to XML
            var xml = JsonConvert.DeserializeXmlNode(jObject.ToString());
            return XamlReader.Parse(xml.InnerXml);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Brush).IsAssignableFrom(objectType);
        }
    }

    class Program
    {
        public static Image fromControlToImage(Size size, System.Windows.FrameworkElement control)
        {
            control.Measure(size);
            control.Arrange(new Rect(size));
            control.UpdateLayout();
            var dpi = 96;
            var dpiV = new Vector2(dpi, dpi);
            var bitmap = new RenderTargetBitmap(
                (int)(size.Width * dpiV.X / 96), (int)(size.Height * dpiV.Y / 96),
                dpiV.X, dpiV.Y, PixelFormats.Pbgra32
            );

            bitmap.Render(control);
            using (var stream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(stream);
                var bitmap1 = new Bitmap(stream);
                return bitmap1;
            }
        }

        static void Main(string[] args)
        {
            var layer = new Layer("coosu default layer");
            layer.Camera2.Scale(12345, 2);
            layer.Camera2.ScaleBy(EasingType.QuartOut, 12345, 15345, 1);
            SpriteGroup textGroup = layer.CreateText("🥰🥵lyric here... owo",
                12345,
                320, 240,
                OriginType.Centre,
                options => options
                    .WithIdentifier("style1")
                    .WithFontFamily("Consolas")
                    .WithFontFamily("SimHei")
                    .WithFontSize(48)
                    .WithWordGap(5)
                    .WithLineGap(10)
                    .ScaleXBy(0.7)
                    .Reverse()
                    .FillBy("#43221415")
                    .WithStroke("#FF131415", 5)
                    .WithShadow("#000000", 5, -60, 4));


            var waitComplete = new TaskCompletionSource<bool>();
            var uiThread = new Thread(() =>
            {
                //var application = new Application
                //{
                //    ShutdownMode = ShutdownMode.OnExplicitShutdown,
                //};

                //application.Startup += (s, e) =>
                //{
                var g = new UserControl1();
                var image = fromControlToImage(new Size(400, 300), g);
                image.Save("D:\\ok.png");
                waitComplete.SetResult(true);
                //    application.Shutdown();
                //};

                //application.Exit += (s, e) => waitComplete.SetResult(true);
                //try
                //{
                //    application.Run();
                //}
                //catch (Exception ex)
                //{
                //    application.Shutdown();
                //}
            })
            {
                IsBackground = true
            };

            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
            waitComplete.Task.Wait();

            var str = "\"\"";
            var split = str.Split('\"');
            var split1 = str.Split(new[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

            var linearGradientBrush = new LinearGradientBrush(Color.FromArgb(3, 3, 2, 5),
                Color.FromArgb(3, 3, 2, 5), 45);
            var coosuTextOptions = new CoosuTextOptions
            {
                ShadowBrush = linearGradientBrush,
            };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            jsonSerializerSettings.Converters.Add(new BrushJsonConverter());
            var json1 = JsonConvert.SerializeObject(coosuTextOptions, jsonSerializerSettings);
            Console.WriteLine(json1);

            var obj = JsonConvert.DeserializeObject<CoosuTextOptions>(json1, jsonSerializerSettings);
        }
    }
}