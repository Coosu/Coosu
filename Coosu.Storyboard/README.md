# Coosu.Storyboard

[![NuGet](https://img.shields.io/nuget/v/Coosu.Storyboard.svg)](https://www.nuget.org/packages/Coosu.Storyboard/)
[![Downloads](https://img.shields.io/nuget/dt/Coosu.Storyboard.svg)](https://www.nuget.org/packages/Coosu.Storyboard/)

**Coosu.Storyboard** is a .NET library designed for creating, parsing, and manipulating osu! storyboard files (`.osb` and `.osu` storyboard sections). It provides a programmatic way to define and control storyboard elements like sprites and animations, along with their various transformations and events over time.

## Features

*   **Create Storyboards Programmatically**: Define sprites, animations, and their behaviors (move, fade, scale, rotate, color) using C# code.
*   **Parsing Existing Storyboards**: Load and parse `.osb` files or storyboard sections within `.osu` files into an object model.
*   **Object-Oriented Structure**: Work with storyboard elements like `Sprite`, `Animation`, `Layer`, and `Scene` in an organized way.
*   **Event System**: Define complex sequences of events (fade, move, scale, rotate, color, parameter changes) with various easing options.
*   **Looping and Triggering**: Create loops and triggers for advanced animation control, as defined in the osu! storyboard specification.
*   **Layer Management**: Organize sprites and animations into different layers (Background, Fail, Pass, Foreground, Overlay).
*   **Extensible**: Designed to be built upon, allowing for custom event types and behaviors (see `Coosu.Storyboard.Extensions` and `Coosu.Storyboard.OsbX`).
*   **High Performance**: Optimized for efficient parsing and generation of storyboard scripts.

## Installation

You can install Coosu.Storyboard via NuGet Package Manager:

```bash
Install-Package Coosu.Storyboard
```

Or using the .NET CLI:

```bash
dotnet add package Coosu.Storyboard
```

## Usage

### Creating a Simple Storyboard

```csharp
using Coosu.Storyboard;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Easing;
using Coosu.Shared.Numerics; // For Vector2D
using System.IO;
using System.Threading.Tasks;

public class StoryboardGenerator
{
    public async Task CreateSampleStoryboardAsync(string outputOsbFile)
    {
        var scene = new Scene();
        var layer = scene.GetOrAddLayer("Foreground"); // Get the Foreground layer, or create if not exists

        // Create a sprite
        var sprite = layer.CreateSprite("sb/sprite.png", LayerType.Foreground, OriginType.Centre);

        // Add events to the sprite
        sprite.Move(0, 1000, 320, 240, 420, 240); // Move from (320,240) to (420,240) between 0ms and 1000ms
        sprite.Fade(0, 1000, 0, 1);          // Fade in from 0ms to 1000ms
        sprite.Scale(0, 1000, 0.5, 1);       // Scale from 0.5 to 1 between 0ms and 1000ms
        sprite.Rotate(0, 1000, 0, Math.PI * 2); // Rotate 360 degrees between 0ms and 1000ms

        // Create an animation
        var animation = layer.CreateAnimation("sb/anim/frame.png", 10, 100, LoopType.LoopForever,
                                            LayerType.Background, OriginType.CentreLeft, 0, 240);
        animation.Fade(1000, 2000, 1, 0); // Fade out animation

        // Add a loop to a sprite
        var loopingSprite = layer.CreateSprite("sb/loop_sprite.png", LayerType.Foreground, OriginType.TopLeft, 0, 0);
        var loop = loopingSprite.StartLoop(2000, 3); // Start loop at 2000ms, repeat 3 times
        {
            loop.Move(EasingType.EaseInQuad, 0, 500, 0, 0, 100, 0); // Relative to loop start time
            loop.Move(EasingType.EaseOutQuad, 500, 1000, 100, 0, 0, 0);
        }
        loopingSprite.EndLoop(); // Or loop.Dispose();

        // Write the storyboard to an .osb file
        using (var writer = new StreamWriter(outputOsbFile))
        {
            await scene.WriteFullScriptAsync(writer);
        }
        Console.WriteLine($"Storyboard saved to: {outputOsbFile}");
    }
}
```

### Parsing an Existing Storyboard

```csharp
using Coosu.Storyboard;
using System.IO;
using System.Threading.Tasks;

public class StoryboardParser
{
    public async Task ParseStoryboardAsync(string osbFilePath)
    {
        Scene scene;
        using (var reader = new StreamReader(osbFilePath))
        {
            scene = await Layer.ParseTextAsync(reader); // Or Scene.ParseTextAsync for a more direct approach
        }

        Console.WriteLine($"Parsed storyboard with {scene.Layers.Count} layers.");

        foreach (var layerEntry in scene.Layers)
        {
            Console.WriteLine($"Layer: {layerEntry.Key} has {layerEntry.Value.SceneObjects.Count} objects.");
            foreach (var sceneObject in layerEntry.Value.SceneObjects)
            {
                Console.WriteLine($"  Object: {sceneObject.ImagePath}, Type: {sceneObject.ObjectType}");
                Console.WriteLine($"    Events: {sceneObject.Events.Count}, Loops: {sceneObject.LoopList.Count}, Triggers: {sceneObject.TriggerList.Count}");
                // Access individual events, loops, triggers
            }
        }
    }
}
```

## Performance

Coosu.Storyboard is optimized for performance. Benchmarks show significant improvements compared to older versions and other libraries.

*(The existing benchmark data from the original README can be kept here, or updated if newer benchmarks are available. Ensure the context of the benchmark (hardware, .NET version) is clear.)*

### Parsing Benchmark
Run `benchmark-OsbParsing.ps1` in the `Benchmarks/OsbParsingBenchmark` directory:
``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1466 (21H2)
Intel Core i7-4770K CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.101
  [Host]             : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET 6.0           : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  .NET Core 3.1      : .NET Core 3.1.22 (CoreCLR 4.700.21.56803, CoreFX 4.700.21.57101), X64 RyuJIT
  .NET Framework 4.8 : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT


```
|      Method |       Runtime |      Mean |     Error |    StdDev | Ratio | RatioSD |    Gen 0 |    Gen 1 | Allocated |
|------------ |-------------- |----------:|----------:|----------:|------:|--------:|---------:|---------:|----------:|
| CoosuLatest |      .NET 6.0 |  1.771 ms | 0.0105 ms | 0.0099 ms |  1.00 |    0.00 |  87.8906 |  42.9688 |    544 KB |
|    CoosuOld |      .NET 6.0 |  8.215 ms | 0.0653 ms | 0.0611 ms |  4.64 |    0.05 | 265.6250 | 125.0000 |  1,626 KB |
|  OsuParsers |      .NET 6.0 | 11.893 ms | 0.1751 ms | 0.1638 ms |  6.72 |    0.10 | 203.1250 |  93.7500 |  1,258 KB |
|             |               |           |           |           |       |         |          |          |           |
| CoosuLatest | .NET Core 3.1 |  2.023 ms | 0.0162 ms | 0.0152 ms |  1.00 |    0.00 |  89.8438 |  42.9688 |    543 KB |
|  OsuParsers | .NET Core 3.1 |  3.388 ms | 0.0267 ms | 0.0250 ms |  1.67 |    0.01 | 207.0313 | 101.5625 |  1,258 KB |
|    CoosuOld | .NET Core 3.1 |  3.697 ms | 0.0736 ms | 0.0723 ms |  1.83 |    0.04 | 265.6250 | 132.8125 |  1,626 KB |
|             |               |           |           |           |       |         |          |          |           |
| CoosuLatest |    .NETFX 4.8 |  3.392 ms | 0.0309 ms | 0.0289 ms |  1.00 |    0.00 | 152.3438 |  74.2188 |    922 KB |
|  OsuParsers |    .NETFX 4.8 |  4.584 ms | 0.0669 ms | 0.0625 ms |  1.35 |    0.02 | 273.4375 | 132.8125 |  1,669 KB |
|    CoosuOld |    .NETFX 4.8 |  4.935 ms | 0.0769 ms | 0.0719 ms |  1.45 |    0.02 | 343.7500 | 171.8750 |  2,102 KB |
