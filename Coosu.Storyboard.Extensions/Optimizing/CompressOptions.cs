using System;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public class CompressOptions
    {
        /// <summary>
        /// Specific value that how many threads can be used while compressing.
        /// <para>The default value is <see cref="Environment.ProcessorCount"/> - 1.</para>
        /// </summary>
        public int ThreadCount { get; set; } = Environment.ProcessorCount == 1 ? 1 : Environment.ProcessorCount - 1;

        /// <summary>
        /// Discretizing interval in milliseconds.
        /// The value should be larger than 16 as recommended in osu!wiki.
        /// <para>The default value is 48.</para>
        /// </summary>
        public int DiscretizingInterval { get; set; } = 48;

        /// <summary>
        /// Discretizing accuracy that how many numbers will be preserved after decimal point.
        /// If the value is null the numbers will not be rounded.
        /// <para>The default value is 3.</para>
        /// </summary>
        public int? DiscretizingAccuracy { get; set; } = 3;
    }
}