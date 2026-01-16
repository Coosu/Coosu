using System;
using System.Linq;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.HitObject;

namespace CoosuDebugConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Current Directory: {Environment.CurrentDirectory}");
            var filename = "Test Artist - Test Title (Test Creator) [New Difficulty].osu";
            if (!System.IO.File.Exists(filename))
            {
                // Try looking in the same directory as the executable
                var exePath = AppContext.BaseDirectory;
                var pathInExeDir = System.IO.Path.Combine(exePath, filename);
                if (System.IO.File.Exists(pathInExeDir))
                {
                    filename = pathInExeDir;
                }
                else
                {
                    Console.WriteLine($"File not found: {filename}");
                    Console.WriteLine($"Also checked: {pathInExeDir}");
                    return;
                }
            }

            var file = OsuFile.ReadFromFile(filename);
            if (file.HitObjects == null || file.HitObjects.HitObjectList.Count == 0)
            {
                Console.WriteLine("No hit objects found.");
                return;
            }

            var hitObject = file.HitObjects.HitObjectList[0];
            var sliderInfo = hitObject.SliderInfo;

            if (sliderInfo == null)
            {
                Console.WriteLine("First object is not a slider.");
                return;
            }

            Console.WriteLine($"Slider Type: {sliderInfo.SliderType}");
            Console.WriteLine($"Start Point: {sliderInfo.StartPoint}");
            Console.WriteLine($"Control Points: {string.Join(", ", sliderInfo.ControlPoints)}");
            Console.WriteLine($"Pixel Length: {sliderInfo.PixelLength}");
            Console.WriteLine($"Slides Count Before Compute: {sliderInfo.SliderType}");

            // The user code uses `extended` which is just `sliderInfo` cast/assigned.
            // But wait, MainWindow.axaml.cs says: `var extended = sliderInfo!;`
            // And then calls `extended.ComputeTicks(120);`
            // Wait, does SliderInfo have ComputeTicks? Or is it an extension method?
            // In Coosu.Beatmap/Sections/HitObject/ExtendedSliderInfo.cs exists? No, I saw ExtendedSliderInfo.cs in the file list.
            // Let's check if SliderInfo inherits from ExtendedSliderInfo or if it's an extension.
            // File list showed: Coosu.Beatmap/Sections/HitObject/ExtendedSliderInfo.cs
            // And Coosu.Beatmap/Sections/HitObject/SliderInfo.cs
            
            // Re-reading MainWindow code:
            // var sliderInfo = file.HitObjects!.HitObjectList[0].SliderInfo;
            // var extended = sliderInfo!;
            // var slides = extended.ComputeTicks(120);
            
            // So sliderInfo IS the object that has ComputeTicks.
            // I should check if ComputeTicks is an extension method or instance method.
            
            try 
            {
                var slides = sliderInfo.ComputeTicks(33);
                Console.WriteLine($"Total Slides: {slides.Length}");
                
                foreach (var slide in slides)
                {
                    Console.WriteLine($"Tick: Time={slide.Offset}, Pos=({slide.Point.X}, {slide.Point.Y})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error computing ticks: {ex}");
            }
        }
    }
}
