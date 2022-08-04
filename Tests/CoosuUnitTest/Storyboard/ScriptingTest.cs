using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Coosu.Storyboard;
using Coosu.Storyboard.Extensions.Optimizing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoosuUnitTest.Storyboard
{
    [TestClass]
    public class ScriptingTest
    {
        [TestMethod]
        public void Loop()
        {
            var group = new Layer();
            var sprite = group.CreateSprite("temp");
            using (var loop = sprite.CreateLoop(12345, 3))
            {
                loop.Move(0, 500, 320, 240, 0, 0);
                loop.Move(500, 1000, 0, 0, 320, 240);
                sprite.Fade(12345, 12345 + 1000, 0, 1);
            }

            var sprite2 = group.CreateSprite("temp");
            sprite2.StartLoop(12345, 3);
            sprite2.Move(0, 500, 320, 240, 0, 0);
            sprite2.Move(500, 1000, 0, 0, 320, 240);
            sprite2.EndLoop();
            sprite2.Fade(12345, 12345 + 1000, 0, 1);

            var str = sprite.ToScriptString();
            var str2 = sprite2.ToScriptString();
            Debug.Assert(str == str2);
        }

        [TestMethod]
        public void CreateElementGroup()
        {
            var group = new Layer(0);
        }

        [TestMethod]
        public void CreateSpriteFromGroup()
        {
            var group = new Layer(0);
            group.CreateSprite("");
            Assert.AreEqual(1, group.SceneObjects.Count);
        }

        [TestMethod]
        public void Parse()
        {
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            Layer.ParseFromFile(
                @"C:\Users\milkitic\Documents\Tencent Files\2241521134\FileRecv\cYsmix_-_triangles\cYsmix - triangles (yf_bmp).osb");
        }

        [TestMethod]
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
}
