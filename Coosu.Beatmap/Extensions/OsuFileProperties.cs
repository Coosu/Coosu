using Coosu.Beatmap.Sections;

namespace Coosu.Beatmap;

public static class OsuFileProperties
{
    extension(OsuFile osuFile)
    {
        public string Filename
        {
            get
            {
                osuFile.Metadata ??= new MetadataSection();
                return osuFile.GetOsuFilename(osuFile.Metadata.Version);
            }
        }
    }
}