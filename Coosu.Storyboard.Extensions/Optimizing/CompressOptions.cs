using System;

namespace Coosu.Storyboard.Extensions.Optimizing;

public class CompressOptions
{
    /// <summary>
    /// Specifies whether repeated command periods and large absolute timings may be represented
    /// by loop commands when the final serialized text becomes shorter.
    /// <para>The default value is true.</para>
    /// </summary>
    public bool ConvertToLoop { get; set; } = true;

    /// <summary>
    /// Specifies whether command families may be rewritten to a lower-dimensional representation
    /// when the final serialized text becomes shorter.
    /// <para>The default value is true.</para>
    /// </summary>
    public bool ReduceCommandDimensions { get; set; } = true;

    /// <summary>
    /// Discretizing accuracy that how many numbers will be preserved after decimal point.
    /// If the value is null the numbers will not be rounded.
    /// <para>The default value is 3.</para>
    /// </summary>
    public int? DiscretizingAccuracy { get; set; } = 3;

    /// <summary>
    /// Discretizing interval in milliseconds.
    /// The value should be larger than 16 as recommended in osu!wiki.
    /// <para>The default value is 48.</para>
    /// </summary>
    public int DiscretizingInterval { get; set; } = 48;

    /// <summary>
    /// Specific auto rounding value. A negative value means no rounding.
    /// <para>The default value is 5.</para>
    /// </summary>
    public int RoundingDecimalPlaces { get; set; } = 5;

    /// <summary>
    /// Specific value that how many threads can be used while compressing.
    /// <para>The default value is <see cref="Environment.ProcessorCount"/> - 1.</para>
    /// </summary>
    public int ThreadCount { get; set; } = Environment.ProcessorCount == 1 ? 1 : Environment.ProcessorCount - 1;
}
