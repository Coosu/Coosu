using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Coosu.Beatmap;
using Coosu.Beatmap.Sections.HitObject;

namespace AvaloniaTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DrawSlider();
        }

        private void DrawSlider()
        {
            var file = OsuFile.ReadFromFile("Test Artist - Test Title (Test Creator) [New Difficulty].osu");
            var sliderInfo = file.HitObjects!.HitObjectList[0].SliderInfo;
            var extended = sliderInfo!;
            //var raw = new RawHitObject { X = 100, Y = 100, Offset = 1000 };
            //var sliderInfo = new SliderInfo(raw);
            //sliderInfo.StartPoint = new Vector3(100, 100, 0);
            //sliderInfo.ControlPoints = new List<Vector3>
            //{
            //    new Vector3(150, 200, 0),
            //    new Vector3(250, 50, 0),
            //    new Vector3(400, 300, 0)
            //};
            //sliderInfo.SliderType = SliderType.Bezier;
            //sliderInfo.PixelLength = 400;
            //sliderInfo.Repeat = 1;
            //sliderInfo.StartTime = 1000;

            //var extended = new ExtendedSliderInfo(sliderInfo, raw);

            // double lastRedFactor, double lastLineMultiple, double diffSliderMultiplier, float diffTickRate
            //extended.UpdateComputedValues(600, 1, 1.4, 1);

            var slides = extended.ComputeTicks(33);

            var canvas = this.FindControl<Canvas>("MainCanvas");

            if (canvas == null) return;

            // Draw Slides
            foreach (var slide in slides)
            {
                var ellipse = new Ellipse
                {
                    Width = 4,
                    Height = 4,
                    Fill = Brushes.Red
                };
                Canvas.SetLeft(ellipse, slide.Point.X - 2);
                Canvas.SetTop(ellipse, slide.Point.Y - 2);
                canvas.Children.Add(ellipse);
            }

            // Draw Anchor Points
            DrawPoint(sliderInfo.StartPoint, Brushes.Green, canvas);
            foreach (var p in sliderInfo.ControlPoints.Where(k => k.Z == 0))
            {
                DrawPoint(p, Brushes.Yellow, canvas);
            }
        }

        private void DrawPoint(Vector3 vec, IBrush brush, Canvas canvas)
        {
            var ellipse = new Ellipse
            {
                Width = 8,
                Height = 8,
                Fill = brush
            };
            Canvas.SetLeft(ellipse, vec.X - 4);
            Canvas.SetTop(ellipse, vec.Y - 4);
            canvas.Children.Add(ellipse);
        }
    }
}