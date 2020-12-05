using System;
using System.Collections.Generic;
using System.Text;

namespace Coosu.Beatmap.Configurable
{
    public abstract class Config
    {
        internal abstract void HandleCustom(string line);

        internal ReadOptions Options { get; set; }
    }
}
