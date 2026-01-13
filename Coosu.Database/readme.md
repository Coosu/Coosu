# Coosu.Database

[![NuGet](https://img.shields.io/nuget/v/Coosu.Database.svg)](https://www.nuget.org/packages/Coosu.Database/)
[![Downloads](https://img.shields.io/nuget/dt/Coosu.Database.svg)](https://www.nuget.org/packages/Coosu.Database/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

**Coosu.Database** is a .NET library for reading and processing osu! database files such as `osu!.db`, `collection.db`, and `scores.db`. It provides tools to parse these binary files and access their structured data.

## Features

*   **Supports Multiple Database Files**: 
    *   `osu!.db`: Contains information about all beatmaps known to the osu! client.
    *   `collection.db`: Stores user-created beatmap collections.
    *   `scores.db`: Holds local scores for beatmaps.
*   **Efficient Parsing**: Designed for performance, especially when dealing with large database files.
*   **Low-level Reader (`OsuDbReader`)**: Provides an `OsuDbReader` class that allows for fine-grained control over the parsing process. This is useful for scenarios where you only need specific pieces of information or want to implement custom object mapping to reduce memory overhead.
*   **High-level Serialization**: Offers convenient methods to deserialize entire database files into strongly-typed object models (e.g., `OsuDb`, `CollectionDb`, `ScoresDb`).
*   **Structured Data Access**: Represents database content with clear C# classes and properties.
*   **Customizable Object Handling**: The `OsuDbReader` enables users to implement their own logic for processing data as it's read, which can be significantly more memory-efficient for large datasets.

## Installation

You can install Coosu.Database via NuGet Package Manager:

```bash
Install-Package Coosu.Database
```

Or using the .NET CLI:

```bash
dotnet add package Coosu.Database
```

## Usage

### Basic Usage: Reading `osu!.db`

This example shows how to read the `osu!.db` file and get the count of beatmaps.

```csharp
using System;
using Coosu.Database.Serialization;

public class OsuDbExample
{
    public void ReadOsuDb(string osuDbPath)
    {
        try
        {
            OsuDb osuDb = OsuDb.ReadFromFile(osuDbPath); // Or OsuDb.ReadFromStream(stream)
            Console.WriteLine($"osu! version: {osuDb.OsuVersion}");
            Console.WriteLine($"Player name: {osuDb.PlayerName}");
            Console.WriteLine($"Total beatmaps: {osuDb.Beatmaps.Count}");

            if (osuDb.Beatmaps.Count > 0)
            {
                Beatmap firstBeatmap = osuDb.Beatmaps[0];
                Console.WriteLine($"First beatmap: {firstBeatmap.Artist} - {firstBeatmap.Title} [{firstBeatmap.Version}]");
                Console.WriteLine($"  Folder: {firstBeatmap.FolderName}, File: {firstBeatmap.BeatmapFileName}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading osu!.db: {ex.Message}");
        }
    }
}
```

### Basic Usage: Reading `collection.db`

```csharp
using System;
using Coosu.Database.Serialization;

public class CollectionDbExample
{
    public void ReadCollectionDb(string collectionDbPath)
    {
        try
        {
            CollectionDb collectionDb = CollectionDb.ReadFromFile(collectionDbPath);
            Console.WriteLine($"osu! version (from collection.db): {collectionDb.OsuVersion}");
            Console.WriteLine($"Total collections: {collectionDb.Collections.Count}");

            foreach (var collection in collectionDb.Collections)
            {
                Console.WriteLine($"Collection: {collection.Name} ({collection.BeatmapCount} beatmaps)");
                // You can iterate through collection.BeatmapMd5Hashes to get MD5 hashes of beatmaps in this collection
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading collection.db: {ex.Message}");
        }
    }
}
```

### Advanced Usage: Using `OsuDbReader` for Custom Beatmap Handling

For large databases, loading everything into memory might be inefficient. `OsuDbReader` allows you to process data as it's being read. This example demonstrates a simplified way to read only specific fields from beatmaps in `osu!.db`.

**1. Define your custom beatmap class:**

```csharp
// MySimpleBeatmap.cs
using Coosu.Database.DataTypes; // For DbGameMode
using System;

public class MySimpleBeatmap
{
    public string? Artist { get; set; }
    public string? Title { get; set; }
    public string? Creator { get; set; }
    public string? Version { get; set; }
    public DbGameMode GameMode { get; set; }
    public string? BeatmapFileName { get; set; }
    public int BeatmapId { get; set; }
    public int BeatmapSetId { get; set; }
}
```

**2. Create an extension method for `OsuDbReader`:**

```csharp
// MyOsuDbReaderExtensions.cs
using Coosu.Database;
using System.Collections.Generic;

public static class MyOsuDbReaderExtensions
{
    public static IEnumerable<MySimpleBeatmap> EnumerateMySimpleBeatmaps(this OsuDbReader reader)
    {
        MySimpleBeatmap? currentBeatmap = null;

        // The `Read()` method advances the reader to the next data node.
        // `reader.NodeType` indicates the type of the current node (e.g., ObjectStart, KeyValue, ObjectEnd).
        // `reader.Name` or `reader.Path` can be used to identify fields (Path is more reliable for uniqueness).
        // `reader.NodeId` provides a numerical ID for the current field, which can be faster for comparison if known.
        while (!reader.IsEndOfStream && reader.Read())
        {
            // Check if we are at the beginning of a Beatmap object within the 'Beatmaps' array
            if (reader.NodeType == NodeType.ObjectStart && reader.Path == "OsuDb.Beatmaps.item")
            {
                currentBeatmap = new MySimpleBeatmap();
                continue;
            }

            // Check if we are at the end of a Beatmap object
            if (reader.NodeType == NodeType.ObjectEnd && reader.Path == "OsuDb.Beatmaps.item" && currentBeatmap != null)
            {
                yield return currentBeatmap;
                currentBeatmap = null; // Reset for the next beatmap
                continue;
            }

            // If we are not inside a beatmap object, or if it's not a KeyValue node, skip
            if (currentBeatmap == null || reader.NodeType != NodeType.KeyValue)
            {
                continue;
            }

            // Fill properties based on the field name (Path provides full context)
            // This is a simplified example. For a full list of NodeIds/Paths, you might need to inspect during debugging.
            switch (reader.Path)
            {
                case "OsuDb.Beatmaps.item.Artist":
                    currentBeatmap.Artist = reader.GetString();
                    break;
                case "OsuDb.Beatmaps.item.Title":
                    currentBeatmap.Title = reader.GetString();
                    break;
                case "OsuDb.Beatmaps.item.Creator":
                    currentBeatmap.Creator = reader.GetString();
                    break;
                case "OsuDb.Beatmaps.item.Version":
                    currentBeatmap.Version = reader.GetString();
                    break;
                case "OsuDb.Beatmaps.item.GameMode":
                    currentBeatmap.GameMode = (Coosu.Database.DataTypes.DbGameMode)reader.GetByte();
                    break;
                case "OsuDb.Beatmaps.item.BeatmapFileName":
                    currentBeatmap.BeatmapFileName = reader.GetString();
                    break;
                case "OsuDb.Beatmaps.item.BeatmapId":
                    currentBeatmap.BeatmapId = reader.GetInt32();
                    break;
                case "OsuDb.Beatmaps.item.BeatmapSetId":
                    currentBeatmap.BeatmapSetId = reader.GetInt32();
                    break;
                // Add more cases for other properties you need
            }
        }
    }
}
```

**3. Use the custom reader:**

```csharp
// In your main code
using Coosu.Database;
using System;
using System.IO;

public class AdvancedOsuDbReaderExample
{
    public void ProcessBeatmapsEfficiently(string osuDbPath)
    {
        try
        {
            using (var fileStream = File.OpenRead(osuDbPath))
            using (var reader = new OsuDbReader(fileStream)) // OsuDbReader is IDisposable
            {
                // Read basic DB info first if needed (Version, PlayerName etc.)
                // This requires careful handling of the reader state or multiple passes if not done strategically.
                // For simplicity, this example focuses on beatmap enumeration.

                Console.WriteLine("Processing beatmaps efficiently...");
                foreach (var beatmap in reader.EnumerateMySimpleBeatmaps()) // Using our extension method
                {
                    Console.WriteLine($"Light Beatmap: {beatmap.Artist} - {beatmap.Title} [{beatmap.Version}], ID: {beatmap.BeatmapId}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with advanced osu!.db reading: {ex.Message}");
        }
    }
}
```

*(The existing benchmark data and detailed custom beatmap handling guide from the original README can be kept here for users who need that level of detail, or updated if newer benchmarks are available. Ensure the context of the benchmark (hardware, .NET version) is clear.)*

## Performance

Coosu.Database is optimized for efficient reading of osu! database files. The `OsuDbReader` allows for custom data handling strategies that can significantly reduce memory allocation and processing time compared to deserializing entire large databases.

### Benchmark
(From original README - shows comparison with 3000 beatmaps database)
```ini

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

