using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Windows.Controls.Canvas;

namespace Coosu.Animation.WPF
{
    public class StoryboardCanvasHost : IDisposable
    {
        internal static readonly Dictionary<ImageSource, Brush> BrushCache = new Dictionary<ImageSource, Brush>();

        public Canvas Canvas { get; }
        protected readonly List<ImageObject> EleList = new List<ImageObject>();

        public StoryboardCanvasHost(Canvas canvas)
        {
            Canvas = canvas;
        }

        public virtual ImageObject CreateElement(FrameworkElement ui,
            Origin<double> origin,
            double width,
            double height,
            int zIndex,
            double defaultX = 320,
            double defaultY = 240)
        {
            //Canvas.Children.Add(ui);

            Panel.SetZIndex(ui, zIndex);
            var ele = new ImageObject(ui, width, height, origin, defaultX, defaultY)
            {
                Host = this
            };

            EleList.Add(ele);
            return ele;
        }

        public virtual void PlayWhole()
        {
            var list = EleList.OrderBy(k => k.MinTime).ToList();
            var sw = Stopwatch.StartNew();
            Task.Run(() =>
            {
                var index = 0;
                while (index < list.Count)
                {
                    while (sw.ElapsedMilliseconds < list[index].MinTime)
                    {
                        Thread.Sleep(1);
                    }

                    var index1 = index;
                    Application.Current?.Dispatcher?.BeginInvoke(new System.Action(() =>
                    {
                        list[index1].BeginAnimation();
                    }));
                    index++;
                }
            });
        }

        public StoryboardGroup CreateStoryboardGroup()
        {
            return new StoryboardGroup(Canvas);
        }

        public int ClearUnusefulElement()
        {
            var tmpList = EleList.Where(k => k.IsFinished).ToList();
            foreach (var imageObject in tmpList)
            {
                imageObject.Dispose();
                EleList.Remove(imageObject);
            }

            return tmpList.Count;
        }

        public void Dispose()
        {
            Canvas.Children.Clear();
            foreach (var imageObject in EleList)
            {
                imageObject?.Dispose();
            }

            BrushCache.Clear();
        }
    }
}