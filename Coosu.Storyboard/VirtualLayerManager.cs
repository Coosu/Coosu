using System;
using System.Collections.Generic;
using System.Text;
using Coosu.Storyboard.Utils;

namespace Coosu.Storyboard
{
    public class VirtualLayerManager
    {
        public Dictionary<double, VirtualLayer> Layers { get; } = new();

        public bool ContainsLayer(double z)
        {
            return Layers.ContainsKey(z);
        }

        public VirtualLayer GetOrAddLayer(double z)
        {
            if (ContainsLayer(z))
                return Layers[z];
            return CreateLayer(z);
        }

        public VirtualLayer CreateLayer(double z)
        {
            var elementGroup = new VirtualLayer(z);
            Layers.Add(z, elementGroup);
            return elementGroup;
        }

        public void AddLayer(VirtualLayer virtualLayer)
        {
            Layers.Add(virtualLayer.ZDistance, virtualLayer);
        }

        public void DeleteLayer(VirtualLayer virtualLayer)
        {
            Layers.Remove(virtualLayer.ZDistance);
        }

        public void DeleteLayer(double z)
        {
            Layers.Remove(z);
        }

        //public static Layer Adjust(Layer layer, double offsetX, double offsetY, int offsetTiming)
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
