namespace Coosu.Beatmap.Configurable
{
    public abstract class Config
    {
        public abstract void HandleCustom(string line);

        public ReadOptions Options { get; set; }
    }
}
