using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Coosu.Shared;

public static class TextWriterExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStandardizedNumber(this TextWriter writer, double d)
    {
        writer.Write(d.ToEnUsFormatString());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStandardizedNumber(this TextWriter writer, float f)
    {
        writer.Write(f.ToEnUsFormatString());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStandardizedNumber(this TextWriter writer, int i)
    {
        writer.Write(i.ToString());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this TextWriter writer, Enum e)
    {
        writer.Write(e.ToString());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task WriteStandardizedNumberAsync(this TextWriter writer, double d)
    {
        await writer.WriteAsync(d.ToEnUsFormatString());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task WriteStandardizedNumberAsync(this TextWriter writer, float f)
    {
        await writer.WriteAsync(f.ToEnUsFormatString());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task WriteStandardizedNumberAsync(this TextWriter writer, int i)
    {
        await writer.WriteAsync(i.ToString());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task WriteAsync(this TextWriter writer, Enum e)
    {
        await writer.WriteAsync(e.ToString());
    }
}