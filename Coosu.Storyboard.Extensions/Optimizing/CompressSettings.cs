using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Extensions.Optimizing
{
    public class CompressSettings
    {
        public int ThreadCount { get; set; } = Environment.ProcessorCount == 1 ? 1 : Environment.ProcessorCount - 1;
        public int DiscretizingInterval { get; set; } = 32;

        /// <summary>
        /// If the value is null the numbers will not be rounded.
        /// </summary>
        public int? DiscretizingAccuracy { get; set; } = 3;
    }
}