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
