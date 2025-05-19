using System;

namespace Coosu.Beatmap.Configurable;

public abstract class Config
{
    public abstract void HandleCustom(ReadOnlyMemory<char> memory);

    public virtual void OnDeserialized()
    {
    }

    [SectionIgnore]
    public ReadOptions Options { get; set; } = null!;
}