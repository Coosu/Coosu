using Coosu.Beatmap.Configurable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoosuUnitTest.Section;

public class SectionTest
{

    private class TestSection : KeyValueSection
    {

        [SectionProperty("TestFloatProperty", UseSpecificFormat = true)]
        public float TestFloatProperty { get; set; } = 114.514f;

        [SectionProperty("TestDoubleProperty", UseSpecificFormat = true)]
        public double TestDoubleProperty { get; set; } = 114.514d;
    }

    private class Test2Section : KeyValueSection
    {
        public Test2Section() : base(CultureInfo.GetCultureInfo("it-IT"))
        {
            
        }

        [SectionProperty("TestFloatProperty", UseSpecificFormat = true)]
        public float TestFloatProperty { get; set; } = 114.514f;

        [SectionProperty("TestDoubleProperty", UseSpecificFormat = true)]
        public double TestDoubleProperty { get; set; } = 114.514d;
    }

    [Fact]
    public async void TestShouldSerializeWithInvariantCultureDefault()
    {
        var section = new TestSection();
        var ms = new MemoryStream();

        using var writer = new StreamWriter(ms);

        section.AppendSerializedString(writer);

        await writer.FlushAsync();
        var buffer = ms.GetBuffer();
        var result = Encoding.UTF8.GetString(buffer);
        result = result[0..result.IndexOf('\0')];

        Assert.Equal(
            "[TestSection]\r\nTestFloatProperty:114.514\r\nTestDoubleProperty:114.514\r\n",
            result);
    }

    [Fact]
    public async void TestShouldSerializeWithInvariantCultureByCtorCulture()
    {
        var section = new Test2Section();
        var ms = new MemoryStream();

        using var writer = new StreamWriter(ms);

        section.AppendSerializedString(writer);

        await writer.FlushAsync();
        var buffer = ms.GetBuffer();
        var result = Encoding.UTF8.GetString(buffer);
        result = result[0..result.IndexOf('\0')];

        Assert.Equal(
            "[Test2Section]\r\nTestFloatProperty:114,514\r\nTestDoubleProperty:114,514\r\n",
            result);
    }
}
