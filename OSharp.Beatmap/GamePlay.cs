using System;

namespace OSharp.Beatmap
{
    public class GamePlay
    {
        private readonly OsuFile _osuFile;

        public GamePlay(OsuFile osuFile)
        {
            _osuFile = osuFile;
        }

        public double MinTime => Math.Min(_osuFile.HitObjects.MinTime, _osuFile.TimingPoints.MinTime);

        public double MaxTime => Math.Max(_osuFile.HitObjects.MaxTime, _osuFile.TimingPoints.MaxTime);

    }
}
