using System;
using System.IO;
using System.Threading.Tasks;

namespace Coosu.Shared
{
    public static class TextWriterExtensions
    {
        public static async Task WriteAsync(this TextWriter writer, double d)
        {
            await writer.WriteAsync(d.ToIcString());
        }

        public static async Task WriteAsync(this TextWriter writer, float f)
        {
            await writer.WriteAsync(f.ToIcString());
        }

        public static async Task WriteAsync(this TextWriter writer, int i)
        {
            await writer.WriteAsync(i.ToString());
        }

        public static async Task WriteAsync(this TextWriter writer, Enum e)
        {
            await writer.WriteAsync(e.ToString());
        }
    }
}