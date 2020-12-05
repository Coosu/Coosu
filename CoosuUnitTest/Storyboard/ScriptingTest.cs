using Microsoft.VisualStudio.TestTools.UnitTesting;
using Coosu.Storyboard.Management;

namespace CoosuUnitTest.Storyboard
{
    [TestClass]
    public class ScriptingTest
    {
        [TestMethod]
        public void CreateElementGroup()
        {
            var group = new ElementGroup(0);
        }

        [TestMethod]
        public void CreateSpriteFromGroup()
        {
            var group = new ElementGroup(0);
            group.CreateSprite("");
            Assert.AreEqual(1, group.ElementList.Count);
        }
    }
}
