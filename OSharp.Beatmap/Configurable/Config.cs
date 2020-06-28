namespace OSharp.Beatmap.Configurable
{
    public abstract class Config
    {
        internal abstract void HandleCustom(string line);

        internal ReadOptions Options { get; set; }
    }
}
