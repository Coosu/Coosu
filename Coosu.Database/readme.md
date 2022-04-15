# Coosu.Database
A library for reading osu database.

**Currently support osu!.db only**

## Basic Usage
```cs
using System;
using Coosu.Database;
using Coosu.Database.Serialization;

var osuDb = OsuDb.ReadFromFile("osu!.db");           
var beatmapCount = osuDb.Beatmaps.Count;
Console.WriteLine(beatmapCount);
```

## Advanced Usage
For the most of time, we only need to get partial information from the database. But most of the databases are very huge, this will lead to memory peak and unnecessary CPU usage caused by a large number of `Beatmap` instance.

So this library introduced `OsuDbReader` class, which allowed users to implement the object handling themselves.

*Default beatmap handling implementation: `Coosu.Database.Serialization.OsuDbReaderExtensions`*

### Benchmark
Benchmark between different libraraies with 3000 beatmaps database (including custom implementation):
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1586 (21H2)
AMD Ryzen 7 5800H with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=6.0.300-preview.22154.4
  [Host]             : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0           : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET Framework 4.8 : .NET Framework 4.8 (4.8.4470.0), X64 RyuJIT


```
| Method       | Runtime            |     Mean |    Error |   StdDev |   Median | Ratio | RatioSD | Allocated |
| ------------ | ------------------ | -------: | -------: | -------: | -------: | ----: | ------: | --------: |
| CoosuCustom  | .NET 6.0           | 26.94 ms | 0.192 ms | 0.170 ms | 26.99 ms |  0.48 |    0.02 |     22 MB |
| CoosuDefault | .NET 6.0           | 55.18 ms | 1.043 ms | 1.743 ms | 54.30 ms |  1.00 |    0.00 |     52 MB |
| OsuParsers   | .NET 6.0           | 58.54 ms | 1.152 ms | 1.759 ms | 58.83 ms |  1.06 |    0.05 |     55 MB |
| Holly's      | .NET 6.0           | 66.82 ms | 1.327 ms | 1.580 ms | 65.69 ms |  1.20 |    0.04 |     60 MB |
|              |                    |          |          |          |          |       |         |           |
| CoosuCustom  | .NET Framework 4.8 | 36.15 ms | 0.181 ms | 0.170 ms | 36.16 ms |  0.52 |    0.02 |     22 MB |
| CoosuDefault | .NET Framework 4.8 | 70.65 ms | 1.412 ms | 2.359 ms | 71.17 ms |  1.00 |    0.00 |     53 MB |
| OsuParsers   | .NET Framework 4.8 | 82.95 ms | 1.587 ms | 1.558 ms | 82.70 ms |  1.20 |    0.06 |     55 MB |
| Holly's      | .NET Framework 4.8 | 95.19 ms | 1.666 ms | 1.477 ms | 95.89 ms |  1.38 |    0.07 |     61 MB |


### Guide for custom beatmap handling

MyBeatmap.cs
```cs
public class SimpleBeatmap
{
    public string Artist { get; set; } = null!;
    public string ArtistUnicode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string TitleUnicode { get; set; } = null!;
    public string Creator { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string Tags { get; set; } = null!;
    public DbGameMode GameMode { get; set; }

    public string AudioFileName { get; set; } = null!;
    public string BeatmapFileName { get; set; } = null!;
    public string FolderName { get; set; } = null!;

    public double DefaultStarRatingStd { get; set; }
    public double DefaultStarRatingTaiko { get; set; }
    public double DefaultStarRatingCtB { get; set; }
    public double DefaultStarRatingMania { get; set; }

    public TimeSpan DrainTime { get; set; }
    public TimeSpan TotalTime { get; set; }
    public TimeSpan AudioPreviewTime { get; set; }
    public int BeatmapId { get; set; }
    public int BeatmapSetId { get; set; }
}
```

SampleOsuDbReaderExtensions.cs
```cs
public static class SampleOsuDbReaderExtensions
{
    public static IEnumerable<SimpleBeatmap> EnumerateTinyBeatmaps(this OsuDbReader reader)
    {
        SimpleBeatmap? beatmap = null;

        while (reader.Read())
        {
            if (reader.NodeType == NodeType.ObjectStart)
            {
                beatmap = new SimpleBeatmap();
                continue;
            }

            if (reader.NodeType == NodeType.ObjectEnd && beatmap != null)
            {
                yield return beatmap;
                beatmap = null;
            }

            if (reader.NodeType == NodeType.ArrayEnd && reader.NodeId == 7) yield break;
            if (beatmap == null) continue;
            if (reader.NodeType is not (NodeType.ArrayStart or NodeType.KeyValue)) continue;
            FillProperty(reader,  beatmap);
        }
    }

    private static void FillProperty(OsuDbReader reader,  SimpleBeatmap beatmap)
    {
        // Use `reader.NodeId` comparision for performance.
        // You can check the full list by debugging
        // `Coosu.Database.Serialization.OsuDbReaderExtensions.EnumerateBeatmaps`,
        // and watch the `BeatmapStructure` field.
        // 
        // For maintainability, use `reader.Name` or `reader.Path`,
        // then `reader.Name.Equals("Artist", StringComparison.Ordinal)`
        var nodeId = reader.NodeId;

        if (nodeId == 9) beatmap.Artist = reader.GetString();
        else if (nodeId == 10) beatmap.ArtistUnicode = reader.GetString();
        else if (nodeId == 11) beatmap.Title = reader.GetString();
        else if (nodeId == 12) beatmap.TitleUnicode = reader.GetString();
        else if (nodeId == 13) beatmap.Creator = reader.GetString();
        else if (nodeId == 14) beatmap.Version = reader.GetString();
        else if (nodeId == 15) beatmap.AudioFileName = reader.GetString();
        else if (nodeId == 17) beatmap.BeatmapFileName = reader.GetString();
        else if (nodeId == 29) SetDefaultStarRating(beatmap, reader, DbGameMode.Circle);
        else if (nodeId == 32) SetDefaultStarRating(beatmap, reader, DbGameMode.Taiko);
        else if (nodeId == 35) SetDefaultStarRating(beatmap, reader, DbGameMode.Catch);
        else if (nodeId == 38) SetDefaultStarRating(beatmap, reader, DbGameMode.Mania);
        else if (nodeId == 40) beatmap.DrainTime = TimeSpan.FromSeconds(reader.GetInt32());
        else if (nodeId == 41) beatmap.TotalTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == 42) beatmap.AudioPreviewTime = TimeSpan.FromMilliseconds(reader.GetInt32());
        else if (nodeId == 46) beatmap.BeatmapId = reader.GetInt32();
        else if (nodeId == 47) beatmap.BeatmapSetId = reader.GetInt32();
        else if (nodeId == 55) beatmap.GameMode = (DbGameMode)reader.GetByte();
        else if (nodeId == 56) beatmap.Source = reader.GetString();
        else if (nodeId == 57) beatmap.Tags = reader.GetString();
        else if (nodeId == 63) beatmap.FolderName = reader.GetString();
    }

    private static void SetDefaultStarRating(SimpleBeatmap beatmap, OsuDbReader osuDbReader, DbGameMode index)
    {
        while (osuDbReader.Read())
        {
            if (osuDbReader.NodeType == NodeType.ArrayEnd) break;
            var data = osuDbReader.GetIntDoublePair();
            var mods = (Mods)data.IntValue;
            if (mods != Mods.None) continue;

            if (index == DbGameMode.Circle) beatmap.DefaultStarRatingStd = data.DoubleValue;
            else if (index == DbGameMode.Taiko) beatmap.DefaultStarRatingTaiko = data.DoubleValue;
            else if (index == DbGameMode.Catch) beatmap.DefaultStarRatingCtB = data.DoubleValue;
            else if (index == DbGameMode.Mania) beatmap.DefaultStarRatingMania = data.DoubleValue;
        }
    }
}
```

