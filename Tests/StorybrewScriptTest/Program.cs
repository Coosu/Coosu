using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Coosu.Storyboard.Advanced.Texting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

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

        static void Main(string[] args)
        {
            var str = "\"\"";
            var split = str.Split('\"');
            var split1 = str.Split(new[] { "\"" }, StringSplitOptions.RemoveEmptyEntries);

            var linearGradientBrush = new LinearGradientBrush(Color.FromArgb(3, 3, 2, 5), Color.FromArgb(3, 3, 2, 5), 45);
            var coosuTextOptions = new CoosuTextOptions()
            {
                ShadowBrush = linearGradientBrush,
            };

            var jsonSerializerSettings = new JsonSerializerSettings()
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
