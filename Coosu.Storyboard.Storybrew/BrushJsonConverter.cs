using System;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coosu.Storyboard.Storybrew;

public class BrushJsonConverter : JsonConverter
{
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