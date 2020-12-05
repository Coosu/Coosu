using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coosu.Beatmap.Configurable
{
    public interface ISerializeWritable
    {
        string ToSerializedString();
        void AppendSerializedString(TextWriter textWriter);
    }
}
