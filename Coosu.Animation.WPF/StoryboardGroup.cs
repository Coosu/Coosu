using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Coosu.Storyboard;

namespace Coosu.Animation.WPF
{
    public class StoryboardGroup : StoryboardCanvasHost
    {
        public System.Windows.Media.Animation.Storyboard Storyboard;

        public StoryboardGroup(Canvas canvas) : base(canvas)
        {
            Storyboard = new System.Windows.Media.Animation.Storyboard();
        }

        public override ImageObject CreateElement(FrameworkElement ui,
            Anchor<double> anchor,
            double width,
            double height,
            int zIndex,
            double defaultX = 320,
            double defaultY = 240)
        {
            Panel.SetZIndex(ui, zIndex);
            //Canvas.Children.Add(ui);
            var ele = new ImageObject(ui, width, height, anchor, defaultX, defaultY, Storyboard)
            {
                Host = this
            };

            EleList.Add(ele);
            return ele;

        }

        public override void PlayWhole()
        {
            //Canvas.Children.Clear();
            Storyboard.Begin();
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
                    Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                    {
                        var imageObject = list[index1];
                        imageObject.AddToCanvas();
                        if (imageObject.ClearAfterFinish)
                        {
                            Storyboard.Completed += (sender, e) => { imageObject.ClearObj(); };
                        }
                        //list[index1].BeginAnimation();
                    }));
                    index++;
                }
            });
        }
    }
}