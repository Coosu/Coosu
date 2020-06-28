using System;
using System.Collections.Generic;
using System.Linq;

namespace OSharp.Api.V1.Beatmap
{
    /// <summary>
    /// Extensional class of beatmap. (NOT API model)
    /// </summary>
    public class OsuBeatmapSet
    {
        /// <summary>
        /// Initialize beatmap-set with beatmaps.
        /// </summary>
        /// <param name="beatmaps"></param>
        public OsuBeatmapSet(IReadOnlyList<OsuBeatmap> beatmaps)
        {
            Beatmaps = beatmaps;
            //SubmitDate = Beatmaps.First().SubmitDate;
            RankedDate = Beatmaps.Any(k => k.ApprovedDate != null && k.Approved == BeatmapApprovedState.Ranked)
                ? Beatmaps.First().ApprovedDate
                : null;
            QualifiedDate = Beatmaps.Any(k => k.ApprovedDate != null && k.Approved == BeatmapApprovedState.Qualified)
                ? Beatmaps.First().ApprovedDate
                : null;
            ApprovedDate = Beatmaps.Any(k => k.ApprovedDate != null && k.Approved == BeatmapApprovedState.Approved)
                ? Beatmaps.First().ApprovedDate
                : null;
            LovedDate = Beatmaps.Any(k => k.ApprovedDate != null && k.Approved == BeatmapApprovedState.Loved)
                ? Beatmaps.First().ApprovedDate
                : null;
            Status = Beatmaps.First().Approved;
            FavouriteCount = Beatmaps.First().FavouriteCount;
            Id = Beatmaps.First().BeatmapSetId;
            Artist = Beatmaps.First().Artist;
            Title = Beatmaps.First().Title;
            CreatorId = Beatmaps.First().CreatorId;
            Creator = Beatmaps.First().Creator;
        }

        /// <summary>
        /// Beatmap-set title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Beatmap-set artist.
        /// </summary>
        public string Artist { get; }

        /// <summary>
        /// Beatmap-set approved state.
        /// </summary>
        public BeatmapApprovedState? Status { get; }

        /// <summary>
        /// Beatmap-set favourite count.
        /// </summary>
        public int? FavouriteCount { get; }

        /// <summary>
        /// Beatmap-set ranked date. (NULL if not ranked.)
        /// </summary>
        public DateTimeOffset? RankedDate { get; }

        /// <summary>
        /// Beatmap-set ranked date. (NULL if not qualified.)
        /// </summary>
        public DateTimeOffset? QualifiedDate { get; }

        /// <summary>
        /// Beatmap-set ranked date. (NULL if not approved.)
        /// </summary>
        public DateTimeOffset? ApprovedDate { get; }

        /// <summary>
        /// Beatmap-set ranked date. (NULL if not loved.)
        /// </summary>
        public DateTimeOffset? LovedDate { get; }

        /// <summary>
        /// Beatmap-set ranked date. (NULL if not submitted.)
        /// </summary>
        public DateTimeOffset? SubmitDate => throw new NotImplementedException();

        /// <summary>
        /// Beatmap-set ID.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Beatmap-set creator's ID.
        /// </summary>
        public long? CreatorId { get; }

        /// <summary>
        /// Beatmap-set creator.
        /// </summary>
        public string Creator { get; }

        /// <summary>
        /// Beatmaps of the beatmap-set.
        /// </summary>
        public IReadOnlyList<OsuBeatmap> Beatmaps { get; }
    }
}
