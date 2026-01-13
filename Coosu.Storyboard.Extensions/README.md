# Coosu Storyboard Extensions

This document covers extensions and advanced functionalities for `Coosu.Storyboard` provided by related libraries.

## Coosu.Storyboard.Extensions

[![NuGet](https://img.shields.io/nuget/v/Coosu.Storyboard.Extensions.svg)](https://www.nuget.org/packages/Coosu.Storyboard.Extensions/)
[![Downloads](https://img.shields.io/nuget/dt/Coosu.Storyboard.Extensions.svg)](https://www.nuget.org/packages/Coosu.Storyboard.Extensions/)

`Coosu.Storyboard.Extensions` enhances the core `Coosu.Storyboard` library by providing useful utilities and advanced event handling capabilities.

### Features

*   **Relative Events**: Define storyboard events (Move, Fade, Scale, Rotate, Color, Vector) relative to the current state of an element, rather than always using absolute values. This simplifies creating complex chained animations.
    *   Example: `sprite.MoveBy(startTime, endTime, 50, 0);` // Moves the sprite 50 pixels to the right from its current position.
*   **Event Computation and Standardization**:
    *   **Discretize Non-Standard Easing**: Converts complex or custom easing functions into a series of linear events, ensuring compatibility with the standard osu! storyboard interpreter.
    *   **Compute Relative Events**: Translates relative events into a sequence of absolute-value standard events for final output.
*   **Storyboard Optimization**: Includes tools and an infrastructure for optimizing storyboard scripts. This can involve:
    *   Merging redundant events.
    *   Removing unnecessary commands.
    *   Reducing file size.
    *(Specific optimization strategies might vary and can be customized.)*
*   **Additional Event Types**: Introduces `MoreEventTypes` like `MoveBy`, `FadeBy`, etc., for easier use of relative transformations.

### Installation

```bash
Install-Package Coosu.Storyboard.Extensions
```

Or using the .NET CLI:

```bash
dotnet add package Coosu.Storyboard.Extensions
```

### Usage Example: Relative Movement and Standardization

```csharp
using Coosu.Storyboard;
using Coosu.Storyboard.Extensions.Computing; // For StandardizeEvents
using System.Threading.Tasks;
using System.IO;

public class StoryboardExtensionsExample
{
    public async Task CreateAdvancedStoryboardAsync(string outputOsbFile)
    {
        var scene = new Scene();
        var layer = scene.GetOrAddLayer("Foreground");
        var sprite = layer.CreateSprite("sb/p.png");

        // Initial state
        sprite.Move(0, 100, 100);
        sprite.Fade(0, 1);

        // Relative movement: move right by 50px, then up by 50px
        sprite.MoveBy(EasingType.QuadOut, 0, 1000, 50, 0);       // Move 50px right over 1 second
        sprite.MoveBy(EasingType.QuadOut, 1000, 2000, 0, -50);   // Then move 50px up over 1 second

        // Relative fade: fade out by 0.5, then fade in by 0.3
        sprite.FadeBy(2000, 3000, -0.5); // Fade out, reducing opacity by 0.5
        sprite.FadeBy(3000, 4000, 0.3);  // Fade in, increasing opacity by 0.3

        // Before writing to file, standardize events to ensure compatibility
        // This will convert relative events and non-standard easings.
        sprite.StandardizeEvents(); 
        // Alternatively, standardize all events in a layer or scene:
        // layer.StandardizeEvents();
        // scene.StandardizeEvents();

        using (var writer = new StreamWriter(outputOsbFile))
        {
            await scene.WriteFullScriptAsync(writer);
        }
        Console.WriteLine($"Advanced storyboard saved to: {outputOsbFile}");
    }
}
```

## Coosu.Storyboard.OsbX

[![NuGet](https://img.shields.io/nuget/v/Coosu.Storyboard.OsbX.svg)](https://www.nuget.org/packages/Coosu.Storyboard.OsbX/)
[![Downloads](https://img.shields.io/nuget/dt/Coosu.Storyboard.OsbX.svg)](https://www.nuget.org/packages/Coosu.Storyboard.OsbX/)

`Coosu.Storyboard.OsbX` introduces an extended storyboard format, `.osbx`, building upon the standard `.osb` format and `Coosu.Storyboard.Extensions`. This extension allows for more complex visual effects and potentially simplifies the creation of sophisticated storyboards.

### Features

*   **Extended Storyboard Format (`.osbx`)**: Provides serialization and deserialization for an extended version of osu! storyboard scripts.
*   **Custom Action Handlers**: Defines new handlers for storyboard actions, allowing for extended or modified behaviors.
    *   Example: `MoveZActionHandler` for controlling Z-axis movement.
    *   Example: `OriginActionHandler` (potentially for more dynamic origin point control).
*   **Custom Subject Handlers**: Introduces new types of storyboard objects or modifies existing ones.
    *   `Camera25Handler`: Suggests support for 2.5D camera manipulations or effects, allowing for depth and perspective changes.
*   **New Event Types**: Introduces OsbX-specific event types like `MoveZ`.
*   **Integration with Coosu.Storyboard.Extensions**: Leverages features from the extensions library.

### Installation

```bash
Install-Package Coosu.Storyboard.OsbX
```

Or using the .NET CLI:

```bash
dotnet add package Coosu.Storyboard.OsbX
```

### Usage Concept (Serialization/Deserialization)

`Coosu.Storyboard.OsbX` provides `OsbxConvert` for serializing `Scene` objects to the `.osbx` string format and deserializing `.osbx` content back into `Scene` objects.

```csharp
using Coosu.Storyboard;
using Coosu.Storyboard.OsbX;
using System.Threading.Tasks;
using System.IO;

public class OsbxExample
{
    public async Task CreateAndSerializeOsbXAsync(string outputOsbxFilePath)
    {
        var scene = new Scene();
        var layer = scene.GetOrAddLayer("Default");
        var sprite = layer.CreateSprite("sb/triangle.png");

        // Utilize standard Coosu.Storyboard features
        sprite.Fade(0, 1000, 0, 1);

        // Utilize OsbX specific features (e.g., MoveZ if you have a Camera25Object)
        var camera = scene.GetOrCreateCamera25Control(); // Helper from OsbXExtensions
        camera.MoveZ(0, 1000, 1, 0.5); // Example: Zooming out
        sprite.MoveZ(0, 1000, 0, 100); // Sprite moves along Z-axis

        // Serialize to .osbx string
        string osbxContent = await OsbxConvert.SerializeObjectAsync(scene);
        File.WriteAllText(outputOsbxFilePath, osbxContent);
        Console.WriteLine($".osbx file saved to: {outputOsbxFilePath}");
    }

    public async Task DeserializeOsbXAsync(string osbxFilePath)
    {
        string osbxContent = File.ReadAllText(osbxFilePath);
        using (var reader = new StringReader(osbxContent))
        {
            Scene scene = await OsbxConvert.DeserializeObjectAsync(reader);
            Console.WriteLine($"Deserialized .osbx with {scene.Layers.Count} layers.");
            // ... further process the scene object ...
        }
    }
}
```

## Contributing

Contributions to enhance these extensions are welcome. Please feel free to open issues or pull requests on the respective GitHub repositories.

## License

Both `Coosu.Storyboard.Extensions` and `Coosu.Storyboard.OsbX` are licensed under the MIT License. See the LICENSE file in their respective project directories for details. 