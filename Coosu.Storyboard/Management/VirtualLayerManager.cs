using System;
using System.Collections.Generic;
using System.Text;

namespace Coosu.Storyboard.Management
{
    public class VirtualLayerManager
    {
        public Dictionary<float, VirtualLayer> Layers { get; } = new();

        public bool ContainsLayer(float z)
        {
            return Layers.ContainsKey(z);
        }

        public VirtualLayer GetOrAddLayer(float z)
        {
            if (ContainsLayer(z))
                return Layers[z];
            return CreateLayer(z);
        }

        public VirtualLayer CreateLayer(float z)
        {
            var elementGroup = new VirtualLayer(z);
            Layers.Add(z, elementGroup);
            return elementGroup;
        }

        public void AddLayer(VirtualLayer layer)
        {
            Layers.Add(layer.ZDistance, layer);
        }

        public void DeleteLayer(VirtualLayer layer)
        {
            Layers.Remove(layer.ZDistance);
        }

        public void DeleteLayer(float z)
        {
            Layers.Remove(z);
        }

        //public static Layer Adjust(Layer layer, float offsetX, float offsetY, int offsetTiming)
        //{
        //    foreach (var obj in layer.Elements)
        //    {
        //        obj.Adjust(offsetX, offsetY, offsetTiming);
        //    }

        //    return layer;
        //}

        public string ToOsbString()
        {
            StringBuilder sb = new();

            foreach (var a in Layers.Values)
            {
                sb.Append(a.ToScriptStringAsync());
            }

            return sb.ToString();
        }

        public void ToOsbFile(string savePath) => System.IO.File.WriteAllText(savePath,
            "[Events]" + Environment.NewLine +
            "//Background and Video events" + Environment.NewLine +
            "//Storyboard Layer 0 (Background)" + Environment.NewLine
            + ToString() +
            "//Storyboard Sound Samples" + Environment.NewLine);

    }
}
