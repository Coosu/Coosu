using System.Globalization;
using Coosu.Storyboard;
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
    }
}
