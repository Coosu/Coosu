using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Coosu.Storyboard;
using StorybrewCommon.Scripting;

namespace StorybrewScriptTest;

class Program
{
    static void Main(string[] args)
    {
        var generatorContext = new MyContext();
        var cls = new TestForError();
        //var cls = new Welcome();
        cls.Generate(generatorContext);
        //new int[3].AsParallel()
        //    .WithDegreeOfParallelism(Environment.ProcessorCount + 1)
        //    .ForAll(k =>
        //    {
        //    });
    }
}

internal class TestForError : StoryboardObjectGenerator
{
    public override void Generate()
    {
        using var layer = this.CreateLayer();
        var sprite = layer.CreateSprite("SDB");
        sprite.MoveX(0, 1000, 30);
        sprite.MoveX(500, 1000, 30);
    }
}