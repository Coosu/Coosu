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
            var group = new VirtualLayer(0);
        }

        [TestMethod]
        public void CreateSpriteFromGroup()
        {
            var group = new VirtualLayer(0);
            group.CreateSprite("");
            Assert.AreEqual(1, group.SceneObjects.Count);
        }
    }
}
