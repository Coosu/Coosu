using System;
using OSharp.Api.V1.Internal;

namespace OSharp.Api.V1.Beatmap
{
    /// <summary>
    /// Beatmap extra metadata and methods.
    /// </summary>
    public class BeatmapExtra
    {
        private readonly OsuBeatmap _beatmap;

        /// <summary>
        /// Initialize beatmap extra class with beatmap.
        /// </summary>
        /// <param name="beatmap">Specified osu beatmap.</param>
        public BeatmapExtra(OsuBeatmap beatmap)
        {
            _beatmap = beatmap;
        }

        /// <summary>
        /// Get thumbnail URI of the beatmap.
        /// </summary>
        public Uri ThumbnailUri => new Uri($"{Link.BeatmapSetThumbUri}{_beatmap.BeatmapSetId}l.jpg");
        /// <summary>
        /// Get URI of the beatmap's beatmap-set page.
        /// </summary>
        public Uri BeatmapSetUri => new Uri($"{Link.BeatmapSetUri}{_beatmap.BeatmapSetId}l.jpg");
        /// <summary>
        /// Get URI of the beatmap page.
        /// </summary>
        public Uri BeatmapUri => new Uri($"{Link.BeatmapUri}{_beatmap.BeatmapId}l.jpg");
        /// <summary>
        /// Get download URI of the map.
        /// </summary>
        public Uri DownloadUri => new Uri($"{Link.BeatmapDownloadUri}{_beatmap.BeatmapSetId}");
        /// <summary>
        /// Get no-video download URI of the map.
        /// </summary>
        public Uri DownloadWithoutVideoUri => new Uri($"{Link.BeatmapDownloadUri}{_beatmap.BeatmapSetId}n");
        /// <summary>
        /// Get osu!direct URI of the map.
        /// </summary>
        public Uri OsuDirectUri => new Uri($"{Link.OsuDirect}{_beatmap.BeatmapSetId}");
    }
}