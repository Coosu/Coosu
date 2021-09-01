using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Storyboard.Common;

namespace Coosu.Storyboard.Extensions.Computing
{
    public static class SpriteHostExtensions
    {
        public static bool ExpandSubHosts(this Layer layer)
        {
            bool hasSubHosts = false;
            foreach (var layerSubHost in layer.SubHosts)
            {
                hasSubHosts = true;
                var sourceIndex = layer.SceneObjects.IndexOf((ISceneObject)layerSubHost);
                layer.SceneObjects.RemoveAt(sourceIndex);
                layer.SceneObjects.InsertRange(sourceIndex, layerSubHost.Sprites);
            }

            return hasSubHosts;
        }
    }
}
