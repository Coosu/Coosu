using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coosu.Storyboard;
using Coosu.Storyboard.Events;

namespace Coosu.Animation.WPF;

/// <summary>
/// MainWindow.xaml 的交互逻辑
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _canvasHost = new StoryboardCanvasHost(MainCanvas);
        //_bitmaps.Add(new BitmapImage(new Uri("C:\\Program Files (x86)\\osu!\\Songs\\1241412 Ariabl'eyeS - Kegare naki Bara Juuji\\SB\\firefly2.png")));
        //_bitmaps.Add(new BitmapImage(new Uri("C:\\Program Files (x86)\\osu!\\Songs\\1241412 Ariabl'eyeS - Kegare naki Bara Juuji\\SB\\waht.png")));
        //_bitmaps.Add(new BitmapImage(new Uri("D:\\游戏资料\\osu thing\\sb\\SB素材\\123.jpg")));

        Stopwatch sw = Stopwatch.StartNew();
        int r = 400;
        int deg = 5;
        double interval = 16.666667 * 3;
        //for (int i = 0; i < 1200; i++)
        //{
        //    var bitmap = _bitmaps[_rnd.Next(_bitmaps.Count)];
        //    var uii = new Image { Source = bitmap };

        //    var ele = _canvasHost.CreateElement(uii, Origins.Center, bitmap.Width, bitmap.Height);

        //    double i1 = i;
        //    ele.ApplyAnimation(k =>
        //    {
        //        //k.Flip(0, 0, FlipMode.FlipX);
        //        k.Blend(0 + interval * i1, 0 + interval * i1, BlendMode.Normal);
        //        k.Color(Easing.Linear,
        //            0 + interval * i1, 1000 + interval * i1,
        //            (255, 255, 128), (128, 128, 128));
        //        k.Color(Easing.Linear,
        //            1000 + interval * i1, 2000 + interval * i1,
        //            (128, 128, 128), (0, 0, 0));

        //        k.ScaleVec(Easing.Linear,
        //            0 + interval * i1, 2000 + interval * i1,
        //            (1, 1), (1.2, 1.2)
        //        );
        //        k.Move(Easing.Linear,
        //            0 + interval * i1, 2000 + interval * i1,
        //            (-107 + (Math.Sin(i1 / 30) + 1) / 2 * 854, 240),
        //            (-107 + (Math.Sin(i1 / 30) + 1) / 2 * 854 + r * Math.Sin(i1 * deg / 180 * Math.PI), 240 + r * Math.Cos(i1 * deg / 180 * Math.PI))
        //        );
        //        //k.Rotate(0, 0, 2000 + _rnd.Next(300),
        //        //    0,
        //        //    Math.PI * 4 * _rnd.NextDouble() - Math.PI * 2);
        //        //
        //    });

        //    _list.Add(ele);
        //}


        //var bitmap = _bitmaps[_rnd.Next(_bitmaps.Count)];
        //var uii = new Image { Source = bitmap };

        //var ele = _canvasHost.CreateElement(uii, Origins.TopLeft, bitmap.Width, bitmap.Height);

        //ele.ApplyAnimation(k =>
        //{
        //    k.Flip(0, 1000, FlipMode.FlipY);
        //    k.Flip(1000, 2000, FlipMode.FlipX);
        //    k.Move(Easing.Linear,
        //        0, 10000,
        //        (-107, 240),
        //        (854, 240)
        //    );
        //});

        //_list.Add(ele);
        var file =
            @"D:\Games\osu!\Songs\161089 Tamura Yukari - MERRY MERRY MERRY MENU Ne!\Tamura Yukari - MERRY MERRY MERRY MENU... Ne! (moonlightleaf).osb";
        var folder = System.IO.Path.GetDirectoryName(file);
        _group = _canvasHost.CreateStoryboardGroup();
        //var folder = @"D:\Games\osu!\Songs\ok";
        var eg = Layer.ParseFromFile(file);

        var min = eg.SceneObjects.Min(k => k.MinTime());
        if (min > 0)
        {
            min = 0;
        }

        int i = int.MinValue;
        foreach (var ec in eg.SceneObjects)
        {
            if (ec is Storyboard.Animation) continue;
            if (ec is not Sprite sprite) continue;
            var imagePath = System.IO.Path.Combine(folder, sprite.ImagePath);
            if (!sprite.ImagePath.Contains("."))
            {
                imagePath += ".jpg";
            }

            var origin = Anchors.FromOriginType(sprite.OriginType);
            var x = sprite.DefaultX;
            var y = sprite.DefaultY;
            if (!System.IO.File.Exists(imagePath))
            {
                var withoutExt = System.IO.Path.GetFileNameWithoutExtension(imagePath);
                var ext = System.IO.Path.GetExtension(imagePath);
                var newFolder = System.IO.Path.GetDirectoryName(imagePath);
                if (ext.ToLower() == ".jpg")
                {
                    imagePath = System.IO.Path.Combine(newFolder, $"{withoutExt}.png");
                }
                else if (ext.ToLower() == ".png")
                {
                    imagePath = System.IO.Path.Combine(newFolder, $"{withoutExt}.jpg");
                }
            }

            if (!System.IO.File.Exists(imagePath)) continue;
            var bitmap = new BitmapImage(new Uri(imagePath));
            _bitmaps.Add(bitmap);
            var container = new Border()
            {
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                Child = new Image
                {
                    Source = bitmap,
                    Height = bitmap.PixelHeight,
                    Width = bitmap.PixelWidth,
                    RenderTransformOrigin = new Point(0.5, 0.5),
                    RenderTransform = new ScaleTransform(1, 1)
                }
            };
            var ele = _group.CreateElement(container, origin, bitmap.PixelWidth, bitmap.PixelHeight, i, x, y);

            ele.ApplyAnimation(k =>
            {
                foreach (var keyEvent in sprite.Events)
                {
                    double startT = keyEvent.StartTime - min, endT = keyEvent.EndTime - min;

                    if (keyEvent.EventType == EventTypes.Move)
                    {
                        var evt = (Move)keyEvent;
                        k.Move((Easing)evt.Easing.GetEasingType(), startT, endT,
                            new Vector2<double>(evt.StartX, evt.StartY),
                            new Vector2<double>(evt.EndX, evt.EndY));
                    }
                    else if (keyEvent.EventType == EventTypes.Fade)
                    {
                        var evt = (Fade)keyEvent;
                        k.Fade((Easing)evt.Easing.GetEasingType(), startT, endT,
                            evt.StartOpacity,
                            evt.EndOpacity);
                    }
                    else if (keyEvent.EventType == EventTypes.Scale)
                    {
                        var evt = (Scale)keyEvent;
                        k.ScaleVec((Easing)evt.Easing.GetEasingType(), startT, endT,
                            new Vector2<double>(evt.StartScale, evt.StartScale),
                            new Vector2<double>(evt.EndScale, evt.EndScale));
                    }
                    else if (keyEvent.EventType == EventTypes.Rotate)
                    {
                        var evt = (Rotate)keyEvent;
                        k.Rotate((Easing)evt.Easing.GetEasingType(), startT, endT,
                            evt.StartRotate,
                            evt.EndRotate);
                    }
                    else if (keyEvent.EventType == EventTypes.Color)
                    {
                        var evt = (Storyboard.Events.Color)keyEvent;
                        k.Color((Easing)evt.Easing.GetEasingType(), startT, endT,
                            new Vector3<double>(evt.StartR, evt.StartG, evt.StartB),
                            new Vector3<double>(evt.EndR, evt.EndG, evt.EndB));
                    }
                    else if (keyEvent.EventType == EventTypes.MoveX)
                    {
                        var evt = (MoveX)keyEvent;
                        k.MoveX((Easing)evt.Easing.GetEasingType(), startT, endT,
                            evt.StartX, evt.EndX);
                    }
                    else if (keyEvent.EventType == EventTypes.MoveY)
                    {
                        var evt = (MoveY)keyEvent;
                        k.MoveY((Easing)evt.Easing.GetEasingType(), startT, endT,
                            evt.StartY, evt.EndY);
                    }
                    else if (keyEvent.EventType == EventTypes.Vector)
                    {
                        var evt = (Storyboard.Events.Vector)keyEvent;
                        k.ScaleVec((Easing)evt.Easing.GetEasingType(), startT, endT,
                            new Vector2<double>(evt.StartScaleX, evt.StartScaleY),
                            new Vector2<double>(evt.EndScaleX, evt.EndScaleY));
                    }
                    else if (keyEvent.EventType == EventTypes.Parameter)
                    {
                        var evt = (Parameter)keyEvent;
                        switch (evt.Type)
                        {
                            case ParameterType.Horizontal:
                                k.Flip(startT, endT, FlipMode.FlipX);
                                break;
                            case ParameterType.Vertical:
                                k.Flip(startT, endT, FlipMode.FlipY);
                                break;
                            case ParameterType.Additive:
                                //k.Blend(startT, endT, BlendMode.Normal);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else if (keyEvent.EventType == EventTypes.Loop)
                    {
                    }
                    else if (keyEvent.EventType == EventTypes.Trigger)
                    {
                    }
                }
            });

            _list.Add(ele);
            i++;
        }

        Console.WriteLine(sw.ElapsedMilliseconds);
    }

    private Random _rnd = new();
    private StoryboardCanvasHost _canvasHost;
    private readonly List<BitmapImage> _bitmaps = new();
    private readonly List<ImageObject> _list = new();
    private StoryboardGroup _group;

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _group.PlayWhole();
    }
}