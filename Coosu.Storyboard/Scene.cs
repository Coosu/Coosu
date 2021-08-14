using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    public class Scene : IScriptable, IAdjustable
    {
        public string Name { get; set; }
        public Dictionary<double, Layer> Layers { get; } = new();

        public Scene(string name = "Scene")
        {
            Name = name;
        }

        public bool ContainsLayer(double z)
        {
            return Layers.ContainsKey(z);
        }

        public Layer GetOrAddLayer(double z)
        {
            if (ContainsLayer(z))
                return Layers[z];
            return CreateLayer(z);
        }

        public Layer CreateLayer(double z)
        {
            var elementGroup = new Layer(z);
            Layers.Add(z, elementGroup);
            return elementGroup;
        }

        public void AddLayer(Layer layer)
        {
            Layers.Add(layer.ZDistance, layer);
        }

        public void DeleteLayer(Layer layer)
        {
            Layers.Remove(layer.ZDistance);
        }

        public void DeleteLayer(double z)
        {
            Layers.Remove(z);
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
