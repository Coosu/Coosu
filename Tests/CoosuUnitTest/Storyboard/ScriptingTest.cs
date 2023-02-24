using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard;
using Coosu.Storyboard.Extensions.Optimizing;
using Coosu.Storyboard.OsbX;
using Xunit;
using Xunit.Abstractions;

namespace CoosuUnitTest.Storyboard;

public class ScriptingTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ScriptingTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task OsbX()
    {
        using (var sr = new StreamReader(@"C:\Users\milki\Desktop\test.osb"))
        {
            var ok = await OsbxConvert.DeserializeObjectAsync(sr);
            var str = await OsbxConvert.SerializeObjectAsync(ok);
        }

        return;
        var osbX = """
            Camera25
             L,1234,5
              MX,0,0,2000,100,3000
             MY,0,1,3000,500,2000
             MZ,0,1,3000,1,2
            Sprite,Background,Centre,"BG.jpg",320,240,0.165345,default
             L,5124,15
              F,0,329600,342725,1
             L,5124,15
              F,0,329600,342725,1
             T,HitNormal,15,12561
              F,0,329600,342725,1
             T,HitNormal,12561,43612
              F,0,329600,342725,1
             M,1,329600,342933,320,380,320,280
             S,1,329600,342933,1.118877,1
            Animation,Background,Centre,"BG.jpg",320,240,2,30,LoopForever,1.12354,default
             L,329600,7
              F,0,329600,342725,1
             L,5124,15
              F,0,329600,342725,1
             T,HitNormal,15,12561
              F,0,329600,342725,1
             T,HitNormal,12561,43612
              F,0,329600,342725,1
             M,1,329600,342933,320,380,320,280
             S,1,329600,342933,1.118877,1
            """;
        using var stringReader = new StringReader(osbX);
        var scene = await OsbxConvert.DeserializeObjectAsync(stringReader);

        var objs = scene.Layers.Values.First().SceneObjects;
        var osbx = await OsbxConvert.SerializeObjectAsync(scene);
    }

    [Fact]
    public async Task Loop()
    {
        var group = new Layer();
        var sprite = group.CreateSprite("temp");
        using (var loop = sprite.CreateLoop(12345, 3))
        {
            loop.Move(0, 500, 320.0123512, 240.01412512, 0, 0);
            loop.Move(500, 1000, 0, 0, 320.0000001, 240.999999);
            sprite.Fade(12345, 12345 + 500, 0, 0.9551235);
            sprite.Fade(12345 + 500, 12345 + 1000, 0.9551235, 0);
            sprite.Fade(12345 + 1000, 12345 + 1500, 0, 0.999945);
            sprite.Fade(12345 + 1500, 12345 + 2000, 0.999945, 0);
            sprite.Fade(12345 + 2000, 12345 + 2500, 0, 0.9551235);
            sprite.Fade(12345 + 2500, 12345 + 3000, 0.9551235, 0);
            sprite.Fade(12345 + 3000, 12345 + 3500, 0, 0.999945);
            sprite.Fade(12345 + 3500, 12345 + 4000, 0.999945, 0);
        }

        var sprite2 = group.CreateSprite("temp");
        sprite2.StartLoop(12345, 3);
        sprite2.Move(0, 500, 320.0123512, 240.01412512, 0, 0);
        sprite2.Move(500, 1000, 0, 0, 320.0000001, 240.999999);
        sprite2.EndLoop();
        sprite2.Fade(12345, 12345 + 500, 0, 0.9551235);
        sprite2.Fade(12345 + 500, 12345 + 1000, 0.9551235, 0);
        sprite2.Fade(12345 + 1000, 12345 + 1500, 0, 0.999945);
        sprite2.Fade(12345 + 1500, 12345 + 2000, 0.999945, 0);
        sprite2.Fade(12345 + 2000, 12345 + 2500, 0, 0.9551235);
        sprite2.Fade(12345 + 2500, 12345 + 3000, 0.9551235, 0);
        sprite2.Fade(12345 + 3000, 12345 + 3500, 0, 0.999945);
        sprite2.Fade(12345 + 3500, 12345 + 4000, 0.999945, 0);

        var str = sprite.ToScriptString();
        var str2 = sprite2.ToScriptString();
        Debug.Assert(str == str2);
        var compressor = new SpriteCompressor(group);
        await compressor.CompressAsync();
        var str3 = sprite.ToScriptString();
        var str4 = sprite2.ToScriptString();
        Debug.Assert(str3 == str4);
    }

    [Fact]
    public async Task RelativeEvent()
    {
        var layer = new Layer(0);


        var sprite = layer.CreateSprite("path");
        sprite.Scale(10000, 0.5);
        sprite.Scale(10400, 0.6);
        sprite.Scale(10500, 0.75);
        sprite.MoveX(10000, 10500, 100, 1000);
        sprite.MoveX(10500, 11000, 1000, 2000);
        sprite.Fade(10000, 0);
        sprite.Fade(10500, 1);
        sprite.Rotate(10000, 11000, 0, Math.PI);

        _testOutputHelper.WriteLine("Before compressing");
        _testOutputHelper.WriteLine(sprite.ToScriptString());

        var compressor = new SpriteCompressor(layer);
        await compressor.CompressAsync();
        _testOutputHelper.WriteLine("After compressing");
        _testOutputHelper.WriteLine(sprite.ToScriptString());
    }

    [Fact]
    public void CreateElementGroup()
    {
        var group = new Layer(0);
    }

    [Fact]
    public void CreateSpriteFromGroup()
    {
        var group = new Layer(0);
        group.CreateSprite("");
        Assert.Equal(1, group.SceneObjects.Count);
    }

    [Fact]
    public void Parse()
    {
        CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
        Layer.ParseFromFile(
            @"C:\Users\milkitic\Documents\Tencent Files\2241521134\FileRecv\cYsmix_-_triangles\cYsmix - triangles (yf_bmp).osb");
    }

    [Fact]
    public async Task Compress()
    {
        var path = @"C:\Users\milkitic\Downloads\406217 Chata - enn\Chata - enn (EvilElvis).osb";
        var layer = await Layer.ParseFromFileAsync(path);
        var compressor = new SpriteCompressor(layer);
        await compressor.CompressAsync();
        var folder = Path.GetDirectoryName(path);
        await using var sw = new StreamWriter(Path.Combine(folder, "compressed.osb"));
        await layer.WriteFullScriptAsync(sw);
    }
}