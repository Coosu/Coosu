using System;
using System.Threading.Tasks;

namespace Coosu.Storyboard.Extensions.Computing
{
    public static class LayerExtensions
    {
        public static async Task ExpandAsync(this Layer eleG)
        {
            await Task.Run(() => { Expand(eleG); });
        }

        public static void ExpandAndFillFadeout(this Layer eleG)
        {
            eleG.InnerFix(true, true);
        }

        public static void Expand(this Layer eleG)
        {
            eleG.InnerFix(true, false);
        }

        public static void FillObsoleteList(this Layer eleG)
        {
            eleG.InnerFix(false, true);
        }

        private static void InnerFix(this Layer eleG, bool expand, bool fillFadeout)
        {
            throw new NotImplementedException();
            if (!expand && !fillFadeout)
                return;
            foreach (var ec in eleG.SceneObjects)
            {
                if (ec is not Sprite ele) continue;

                if (expand) ele.Expand();
                if (fillFadeout) ele.ComputeInvisibleRange(out _);
            }
        }
    }
}
