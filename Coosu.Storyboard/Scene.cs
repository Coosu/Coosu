using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard
{
    public class Scene : IScriptable, IAdjustable
    {
        public string Name { get; set; }
        public Dictionary<string, Layer> Layers { get; } = new();

        public Scene(string name = "Scene")
        {
            Name = name;
        }

        public bool ContainsLayer(string cameraId)
        {
            return Layers.ContainsKey(cameraId);
        }

        public Layer GetOrAddLayer(string cameraId)
        {
            if (ContainsLayer(cameraId))
                return Layers[cameraId];
            return CreateLayer(cameraId);
        }

        public Layer CreateLayer(string cameraId)
        {
            var elementGroup = new Layer(cameraId);
            Layers.Add(cameraId, elementGroup);
            return elementGroup;
        }

        public void AddLayer(Layer layer)
        {
            Layers.Add(layer.Camera2.CameraIdentifier, layer);
        }

        public void DeleteLayer(Layer layer)
        {
            Layers.Remove(layer.Camera2.CameraIdentifier);
        }

        public void DeleteLayer(string cameraId)
        {
            Layers.Remove(cameraId);
        }

        public async Task WriteHeaderAsync(TextWriter writer)
        {
            await writer.WriteAsync("Scene: ");
            await writer.WriteAsync(Name);
        }

        public async Task WriteScriptAsync(TextWriter writer)
        {
            foreach (var obj in Layers.OrderByDescending(k => k.Key))
            {
                await obj.Value.WriteScriptAsync(writer);
            }
        }

        public async Task WriteFullScriptAsync(TextWriter writer)
        {
            await writer.WriteLineAsync("[Events]");
            await writer.WriteLineAsync("//Background and Video events");
            await writer.WriteLineAsync("//Storyboard Layer 0 (Background)");
            await writer.WriteLineAsync("//Storyboard Layer 1 (Fail)");
            await writer.WriteLineAsync("//Storyboard Layer 2 (Pass)");
            await writer.WriteLineAsync("//Storyboard Layer 3 (Foreground)");
            await WriteScriptAsync(writer);
            await writer.WriteLineAsync("//Storyboard Layer 4 (Overlay)");
            await writer.WriteLineAsync("//Storyboard Sound Samples");
        }

        public void AdjustTiming(double offset)
        {
            foreach (var layer in Layers.Values)
            {
                layer.AdjustTiming(offset);
            }
        }

        public void AdjustPosition(double x, double y)
        {
            foreach (var layer in Layers.Values)
            {
                layer.AdjustPosition(x, y);
            }
        }
    }
}
