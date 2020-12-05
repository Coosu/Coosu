using Coosu.Shared.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coosu.Beatmap
{
    public class OsuFileAnalyzer
    {
        public OsuFile OsuFile { get; }
        public GamePlay GamePlay { get; }

        public string OsbFileName => File.EscapeFileName(string.Format("{0} - {1} ({2}).osb"
            , OsuFile.Metadata.Artist,
            OsuFile.Metadata.Title,
            OsuFile.Metadata.Creator));

        public string FileName => OsuFile.ToString();

        public OsuFileAnalyzer(OsuFile osuFile)
        {
            OsuFile = osuFile;
            GamePlay = new GamePlay(osuFile);
        }
    }
}
