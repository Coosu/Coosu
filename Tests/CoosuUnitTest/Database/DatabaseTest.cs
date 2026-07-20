using System.IO;
using System.Linq;
using Coosu.Database;
using Coosu.Database.Serialization;
using Xunit;

namespace CoosuUnitTest.Database;

public class DatabaseTest
{
    private const string OsuDbPath = @"E:\其他文件\osu!\osu!.db";

    [Fact]
    public void ReadFromFile_ShouldParseHardcodedOsuDb()
    {
        Assert.True(File.Exists(OsuDbPath), $"Test osu!.db not found: {OsuDbPath}");

        var osuDb = OsuDb.ReadFromFile(OsuDbPath);

        Assert.True(osuDb.OsuVersion > 0);
        Assert.True(osuDb.FolderCount >= 0);
        Assert.False(string.IsNullOrWhiteSpace(osuDb.PlayerName));
        Assert.NotEmpty(osuDb.Beatmaps);

        var beatmap = osuDb.Beatmaps.First();
        Assert.False(string.IsNullOrWhiteSpace(beatmap.Artist));
        Assert.False(string.IsNullOrWhiteSpace(beatmap.Title));
        Assert.False(string.IsNullOrWhiteSpace(beatmap.Creator));
        Assert.False(string.IsNullOrWhiteSpace(beatmap.Difficulty));
        Assert.False(string.IsNullOrWhiteSpace(beatmap.FileName));
        Assert.True(beatmap.TotalTime >= beatmap.DrainTime);
    }

    [Fact]
    public void EnumerateBeatmaps_ShouldMatchReadFromFileBeatmapCount()
    {
        Assert.True(File.Exists(OsuDbPath), $"Test osu!.db not found: {OsuDbPath}");

        var osuDb = OsuDb.ReadFromFile(OsuDbPath);

        using var reader = new OsuDbReader(OsuDbPath);
        var beatmapCount = reader.EnumerateBeatmaps().Count();

        Assert.Equal(osuDb.Beatmaps.Count, beatmapCount);
    }
}
