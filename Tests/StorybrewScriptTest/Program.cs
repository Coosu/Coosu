using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace StorybrewScriptTest
{

    class Program
    {
        static void Main(string[] args)
        {
            var generatorContext = new MyContext();
            var cls = new MyTestEffect();
            cls.Generate(generatorContext);
            //new int[3].AsParallel()
            //    .WithDegreeOfParallelism(Environment.ProcessorCount + 1)
            //    .ForAll(k =>
            //    {
            //    });
        }
    }
}