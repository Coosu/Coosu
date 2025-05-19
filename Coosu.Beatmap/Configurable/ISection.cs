using System;

namespace Coosu.Beatmap.Configurable;

public interface ISection : ISerializeWritable
{
    void Match(ReadOnlyMemory<char> memory);
}