using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Coosu.Storyboard;
using SysAni = System.Windows.Media.Animation;

namespace Coosu.Animation.WPF;

public class ImageObject : TransformableObject<double>, IDisposable
{
    public void Dispose()
    {
        _image = null;
        _addImage = null;
        _transformList.Clear();
        LoopList.Clear();
        _blendDurationQueue.Clear();
    }

    private FrameworkElement _image;
    private Rectangle _addImage;

    private readonly List<(DependencyProperty prop, AnimationTimeline animation)> _transformList =
        new List<(DependencyProperty, AnimationTimeline)>();

    internal readonly SysAni.Storyboard Storyboard;
    internal readonly List<SysAni.Storyboard> LoopList = new List<SysAni.Storyboard>();
    private AnimateStatus _blendStatus;
    //private AnimateStatus _flipStatus;
    private readonly Queue<Vector2<double>> _blendDurationQueue = new Queue<Vector2<double>>();
    private DispatcherTimer _dispatcherT;
    private SolidColorBrush _coverBrush;
    private TransformGroup _group;
    private double _defaultX;
    private double _defaultY;
    private bool _groupMode;

    private double? _firstFade;
    private double? _firstRotate;
    private double? _firstMX;
    private double? _firstMY;
    private double? _firstVX;
    private double? _firstVY;
    private int _firstFadeIndex;

    private double PlayTime => Storyboard.GetCurrentTime().TotalMilliseconds;

    public Anchor<double> Anchor { get; }

    public double OriginHeight { get; }

    public double OriginWidth { get; }

    public bool ClearAfterFinish { get; set; } = true;

    internal StoryboardCanvasHost Host { get; set; }
    public bool IsFinished { get; set; } = false;

    internal ImageObject(FrameworkElement image, double width, double height, Anchor<double> anchor, double defaultX,
        double defaultY, SysAni.Storyboard storyboard = null)
    {
        _image = image;
        Anchor = anchor;
        _image.RenderTransformOrigin = new Point(anchor.X, anchor.Y);

        _image.RenderTransform = InitTransformGroup(width, height, defaultX, defaultY);

        if (storyboard != null)
        {
            _groupMode = true;
        }

        Storyboard = storyboard ?? new SysAni.Storyboard();
        //Storyboard.CurrentTimeInvalidated += Storyboard_CurrentTimeInvalidated;
        OriginWidth = width;
        OriginHeight = height;
    }

    //private void Storyboard_CurrentTimeInvalidated(object sender, EventArgs e)
    //{
    //    var o = sender as ClockGroup;
    //    PlayTime = (o?.CurrentTime ?? TimeSpan.Zero).TotalMilliseconds;
    //}

    private TransformGroup InitTransformGroup(double width, double height, double defaultX, double defaultY)
    {
        _defaultX = defaultX;
        _defaultY = defaultY;

        if (_group == null)
        {
            var scaleTransform = new ScaleTransform(1, 1);
            var rotateTransform = new RotateTransform(0);
            var flipTransform = new ScaleTransform(1, 1);
            var translateTransform =
                new TranslateTransform(defaultX - width * Anchor.X, defaultY - height * Anchor.Y);

            _group = new TransformGroup();
            _group.Children.Add(scaleTransform);
            _group.Children.Add(rotateTransform);
            _group.Children.Add(flipTransform);
            _group.Children.Add(translateTransform);
        }

        return _group;
    }

    public void BeginAnimation()
    {
        Storyboard.Completed += (sender, e) =>
        {
            Console.WriteLine("finished");
            IsFinished = true;
        };
        Storyboard.Begin();
        if (_blendStatus == AnimateStatus.Dynamic)
        {
            Task.Run(() =>
            {
                var behavior = FillBehavior.HoldEnd;

                Application.Current.Dispatcher?.Invoke(() => { behavior = Storyboard.FillBehavior; });
                while (behavior != FillBehavior.Stop && _blendDurationQueue.Count > 0)
                {
                    var now = _blendDurationQueue.Peek();
                    if (PlayTime < now.X)
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(now.X - PlayTime));
                    }

                    if (PlayTime >= now.X && PlayTime <= now.Y)
                    {
                        Application.Current?.Dispatcher?.Invoke(new Action(() =>
                        {
                            _image.Visibility = Visibility.Hidden;
                            _addImage.Visibility = Visibility.Visible;
                        }), null);
                        Console.WriteLine("show");
                        Thread.Sleep(TimeSpan.FromMilliseconds(now.Y - PlayTime));
                        Application.Current?.Dispatcher?.Invoke(new Action(() =>
                        {
                            _image.Visibility = Visibility.Visible;
                            _addImage.Visibility = Visibility.Hidden;
                        }), null);
                        _blendDurationQueue.Dequeue();
                        Console.WriteLine("hidden");
                    }

                    Application.Current.Dispatcher?.Invoke(() => { behavior = Storyboard.FillBehavior; });

                    Thread.Sleep(20);
                }
            });
        }
        //_dispatcherT = new DispatcherTimer
        //{
        //    Interval = TimeSpan.FromMilliseconds(MinTime)
        //};

        //_dispatcherT.Start();
        //_dispatcherT.Tick += (obj, args) =>
        //{


        //    _dispatcherT.Stop();
        //};
    }

    public void Reset()
    {
        _image.RenderTransform = InitTransformGroup(OriginWidth, OriginHeight, _defaultX, _defaultY);
        if (_addImage != null)
        {
            _addImage.RenderTransform = InitTransformGroup(OriginWidth, OriginHeight, _defaultX, _defaultY);
        }

        _dispatcherT?.Stop();
    }

    protected override void FadeAction(List<TransformAction> actions)
    {
        if (_firstFade == null)
        {
            _firstFade = (double)actions.First().StartParam;
            _firstFadeIndex = _transformList.Count;
        }

        foreach (var transformAction in actions)
        {
            var dur = TimeSpan.FromMilliseconds(transformAction.EndTime - transformAction.StartTime);
            var duration = new Duration(dur < TimeSpan.Zero ? TimeSpan.Zero : dur);
            _transformList.Add((UIElement.OpacityProperty, new DoubleAnimation
            {
                From = (double)transformAction.StartParam,
                To = (double)transformAction.EndParam,
                EasingFunction = ConvertEasing(transformAction.Easing),
                BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                Duration = duration
            }));
        }
    }

    protected override void RotateAction(List<TransformAction> actions)
    {
        if (_firstRotate == null)
        {
            _firstRotate = (double)actions.First().StartParam / Math.PI * 180;
        }

        foreach (var transformAction in actions)
        {
            var dur = TimeSpan.FromMilliseconds(transformAction.EndTime - transformAction.StartTime);
            var duration = new Duration(dur < TimeSpan.Zero ? TimeSpan.Zero : dur);
            _transformList.Add((RotateTransform.AngleProperty, new DoubleAnimation
            {
                From = (double)transformAction.StartParam / Math.PI * 180,
                To = (double)transformAction.EndParam / Math.PI * 180,
                EasingFunction = ConvertEasing(transformAction.Easing),
                BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                Duration = duration
            }));
        }
    }

    protected override void MoveAction(List<TransformAction> actions)
    {
        if (_firstMX == null)
        {
            _firstMX = ((Vector2<double>)(actions.First().StartParam)).X - OriginWidth * Anchor.X;
        }

        if (_firstMY == null)
        {
            _firstMY = ((Vector2<double>)(actions.First().StartParam)).Y - OriginHeight * Anchor.Y;
        }

        foreach (var transformAction in actions)
        {
            var dur = TimeSpan.FromMilliseconds(transformAction.EndTime - transformAction.StartTime);
            var duration = new Duration(dur < TimeSpan.Zero ? TimeSpan.Zero : dur);
            var startP = (Vector2<double>)transformAction.StartParam;
            var endP = (Vector2<double>)transformAction.EndParam;
            _transformList.Add((TranslateTransform.XProperty, new DoubleAnimation
            {
                From = startP.X - OriginWidth * Anchor.X,
                To = endP.X - OriginWidth * Anchor.X,
                EasingFunction = ConvertEasing(transformAction.Easing),
                BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                Duration = duration
            }));
            _transformList.Add((TranslateTransform.YProperty, new DoubleAnimation
            {
                From = startP.Y - OriginHeight * Anchor.Y,
                To = endP.Y - OriginHeight * Anchor.Y,
                EasingFunction = ConvertEasing(transformAction.Easing),
                BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                Duration = duration
            }));
        }
    }

    protected override void Move1DAction(List<TransformAction> actions, bool isHorizon)
    {
        if (_firstMX == null && isHorizon)
        {
            _firstMX = (double)(actions.First().StartParam) - OriginWidth * Anchor.X;
        }
        else if (_firstMY == null && !isHorizon)
        {
            _firstMY = (double)(actions.First().StartParam) - OriginHeight * Anchor.Y;
        }

        foreach (var transformAction in actions)
        {
            var dur = TimeSpan.FromMilliseconds(transformAction.EndTime - transformAction.StartTime);
            var duration = new Duration(dur < TimeSpan.Zero ? TimeSpan.Zero : dur);
            var startP = (double)transformAction.StartParam;
            var endP = (double)transformAction.EndParam;
            if (isHorizon)
            {
                _transformList.Add((TranslateTransform.XProperty, new DoubleAnimation
                {
                    From = startP - OriginWidth * Anchor.X,
                    To = endP - OriginWidth * Anchor.X,
                    EasingFunction = ConvertEasing(transformAction.Easing),
                    BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                    Duration = duration
                }));
            }
            else
            {
                _transformList.Add((TranslateTransform.YProperty, new DoubleAnimation
                {
                    From = startP - OriginHeight * Anchor.Y,
                    To = endP - OriginHeight * Anchor.Y,
                    EasingFunction = ConvertEasing(transformAction.Easing),
                    BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                    Duration = duration
                }));
            }
        }
    }

    protected override void ScaleVecAction(List<TransformAction> actions)
    {
        if (_firstVX == null)
        {
            _firstVX = ((Vector2<double>)actions.First().StartParam).X;
        }

        if (_firstVY == null)
        {
            _firstVY = ((Vector2<double>)actions.First().StartParam).Y;
        }

        foreach (var transformAction in actions)
        {
            var dur = TimeSpan.FromMilliseconds(transformAction.EndTime - transformAction.StartTime);
            var duration = new Duration(dur < TimeSpan.Zero ? TimeSpan.Zero : dur);
            var startP = (Vector2<double>)transformAction.StartParam;
            var endP = (Vector2<double>)transformAction.EndParam;

            _transformList.Add((ScaleTransform.ScaleXProperty, new DoubleAnimation
            {
                From = startP.X,
                To = endP.X,
                EasingFunction = ConvertEasing(transformAction.Easing),
                BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                Duration = duration
            }));
            _transformList.Add((ScaleTransform.ScaleYProperty, new DoubleAnimation
            {
                From = startP.Y,
                To = endP.Y,
                EasingFunction = ConvertEasing(transformAction.Easing),
                BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                Duration = duration
            }));
        }
    }

    protected override void ColorAction(List<TransformAction> actions)
    {
        var dv = new DrawingVisual();
        _coverBrush = new SolidColorBrush(Colors.White);
        using (var dc = dv.RenderOpen())
        {
            dc.DrawRectangle(_coverBrush, null, new Rect(new Size(1, 1)));
        }

        _image.Effect = new BlendEffect
        {
            Mode = BlendModes.Multiply,
            Amount = 1,
            Blend = new VisualBrush
            {
                Visual = dv
            }
        };
        //var sb = ((GeometryDrawing)((DrawingVisual)((VisualBrush)((BlendEffect)_image.Effect).Blend).Visual).Drawing
        //    .Children[0]).Brush;
        foreach (var transformAction in actions)
        {
            var dur = TimeSpan.FromMilliseconds(transformAction.EndTime - transformAction.StartTime);
            var duration = new Duration(dur < TimeSpan.Zero ? TimeSpan.Zero : dur);
            var startP = (Vector3<double>)transformAction.StartParam;
            var endP = (Vector3<double>)transformAction.EndParam;
            _transformList.Add((SolidColorBrush.ColorProperty, new ColorAnimation
            {
                From = Color.FromRgb((byte)startP.X, (byte)startP.Y, (byte)startP.Z),
                To = Color.FromRgb((byte)endP.X, (byte)endP.Y, (byte)endP.Z),
                EasingFunction = ConvertEasing(transformAction.Easing),
                BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                Duration = duration
            }));
        }
    }

    protected override void BlendAction(List<TransformAction> actions)
    {
        //var source = _image.Source;
        //if (!StoryboardCanvasHost.BrushCache.ContainsKey(source))
        //{
        //    StoryboardCanvasHost.BrushCache.Add(source, new VisualBrush { Visual = _image });
        //}

        _addImage = new Rectangle
        {
            Fill = Brushes.Transparent,
            Width = OriginWidth,
            Height = OriginHeight,
            Effect = new BlendEffect
            {
                Mode = BlendModes.Normal,
                Amount = 1,
                //Blend = StoryboardCanvasHost.BrushCache[source],
                Blend = new VisualBrush { Visual = _image }
            },
            RenderTransform = InitTransformGroup(OriginWidth, OriginHeight, _defaultX, _defaultY),
            RenderTransformOrigin = new Point(Anchor.X, Anchor.Y)
        };

        foreach (var transformAction in actions)
        {
            _blendDurationQueue.Enqueue(new Vector2<double>(transformAction.StartTime - (_groupMode ? 0 : MinTime),
                transformAction.EndTime - (_groupMode ? 0 : MinTime)));
        }

        _blendStatus = AnimateStatus.Static;
        return;
        if (actions.Count == 1)
        {
            var o = actions[0];
            if (o.StartTime.Equals(o.EndTime))
            {
                _image.Visibility = Visibility.Hidden;
                _addImage.Visibility = Visibility.Visible;
                _blendStatus = AnimateStatus.Static;
            }
            else
            {
                _blendStatus = AnimateStatus.Dynamic;
            }
        }
        else
        {
            _blendStatus = AnimateStatus.Dynamic;
        }
    }

    protected override void FlipAction(List<TransformAction> actions)
    {
        //    if (_image.RenderTransform is TransformGroup g)
        //    {
        //        g.Children.Add(new ScaleTransform(1, 1));
        //    }
        var dic = new Dictionary<FlipMode, AnimateStatus>()
        {
            [FlipMode.FlipX] = AnimateStatus.None,
            [FlipMode.FlipY] = AnimateStatus.None
        };

        var groupBy = actions.GroupBy(k => (FlipMode)k.EndParam)
            .ToDictionary(k => k.Key, k => k.ToList());
        foreach (var kvp in groupBy)
        {
            if (kvp.Value.Count > 1)
            {
                dic[kvp.Key] = AnimateStatus.Dynamic;
            }
            else if (kvp.Value[0].StartTime.Equals(kvp.Value[0].EndTime))
            {
                dic[kvp.Key] = AnimateStatus.Static;
            }
            else
            {
                dic[kvp.Key] = AnimateStatus.Dynamic;
            }
        }

        //if (actions.Count == 1)
        //{
        //    var o = actions[0];
        //    var flipMode = (FlipMode)transformAction.EndParam;
        //    flipStatus = o.StartTime.Equals(o.EndTime) ? AnimateStatus.Static : AnimateStatus.Dynamic;
        //}
        //else if (actions.Count == 2 && actions[0].EndParam != actions[1].EndParam)
        //{

        //}
        //else
        //{
        //    flipStatus = AnimateStatus.Dynamic;
        //}

        foreach (var transformAction in actions)
        {
            var flip = (FlipMode)transformAction.EndParam;
            DependencyProperty prop = null;

            if (flip == FlipMode.FlipX)
            {
                prop = SkewTransform.AngleXProperty;
            }
            else if (flip == FlipMode.FlipY)
            {
                prop = SkewTransform.AngleYProperty;
            }

            var dur = TimeSpan.FromMilliseconds(transformAction.EndTime - transformAction.StartTime);
            var duration = new Duration(dur < TimeSpan.Zero ? TimeSpan.Zero : dur);

            _transformList.Add((prop, new DoubleAnimation
            {
                From = 1,
                To = -1,
                BeginTime = TimeSpan.FromMilliseconds(transformAction.StartTime - (_groupMode ? 0 : MinTime)),
                Duration = TimeSpan.Zero
            }));

            if (dic[flip] == AnimateStatus.Dynamic)
            {
                _transformList.Add((prop, new DoubleAnimation
                {
                    From = 1,
                    To = 1,
                    BeginTime = TimeSpan.FromMilliseconds(transformAction.EndTime - (_groupMode ? 0 : MinTime)),
                    Duration = TimeSpan.Zero
                }));
            }
        }
    }

    protected override void HandleLoopGroup(
        (double startTime, int loopTimes, ITransformable<double> transformList) tuple)
    {
        var (startTime, loopTimes, transformList) = tuple;
        LoopList.Add(new SysAni.Storyboard());
        if (transformList is InternalTransformableObject<double> sb)
        {
        }
    }

    protected override void StartAnimation()
    {
    }

    protected override void EndAnimation()
    {
        if (_groupMode)
        {
            _image.Opacity = 0;

            _transformList.Insert(_firstFadeIndex, (UIElement.OpacityProperty, new DoubleAnimation
            {
                From = _firstFade ?? 1,
                To = _firstFade ?? 1,
                BeginTime = TimeSpan.FromMilliseconds(_groupMode ? MinTime : 0),
                Duration = TimeSpan.Zero
            }));


            var scaTrans = (ScaleTransform)_group.Children[0];
            var rotTrans = (RotateTransform)_group.Children[1];
            var transTrans = (TranslateTransform)_group.Children[3];
            scaTrans.ScaleX = _firstVX ?? 1;
            scaTrans.ScaleY = _firstVY ?? 1;

            rotTrans.Angle = _firstRotate ?? 0;

            transTrans.X = _firstMX ?? _defaultX - OriginWidth * Anchor.X;
            transTrans.Y = _firstMY ?? _defaultY - OriginHeight * Anchor.Y;

            if (ClearAfterFinish)
            {
                _transformList.Add((UIElement.OpacityProperty, new DoubleAnimation
                {
                    From = 0,
                    To = 0,
                    BeginTime = TimeSpan.FromMilliseconds(MaxTime),
                    Duration = TimeSpan.Zero
                }));
            }
        }

        if (_transformList.Count(k => k.prop == SkewTransform.AngleXProperty ||
                                      k.prop == SkewTransform.AngleYProperty) > 1)
        {

        }

        foreach (var (prop, animation) in _transformList)
        {
            //if (prop == SolidColorBrush.ColorProperty)
            //{
            //    continue;
            //}
            if (animation.Duration < TimeSpan.Zero)
            {
            }

            AnimationTimeline copy = null;
            if (prop != SkewTransform.AngleXProperty && prop != SkewTransform.AngleYProperty)
                switch (_blendStatus)
                {
                    case AnimateStatus.None:
                        SysAni.Storyboard.SetTarget(animation, _image);
                        break;
                    case AnimateStatus.Static when prop != SolidColorBrush.ColorProperty:
                        SysAni.Storyboard.SetTarget(animation, _addImage);
                        break;
                    case AnimateStatus.Dynamic:
                        copy = animation.Clone();
                        SysAni.Storyboard.SetTarget(copy, _addImage);
                        SysAni.Storyboard.SetTarget(animation, _image);

                        Storyboard.Children.Add(copy);
                        break;
                    //default:
                    //    throw new ArgumentOutOfRangeException();
                }
            else
            {
                SysAni.Storyboard.SetTarget(animation, ((Border)(_image)).Child);
            }

            Storyboard.Children.Add(animation);

            if (prop == RotateTransform.AngleProperty)
            {
                SysAni.Storyboard.SetTargetProperty(animation,
                    new PropertyPath("RenderTransform.Children[1].Angle"));
                //SetProp("RenderTransform.Children[1].Angle", animation, copy);
            }
            else if (prop == TranslateTransform.XProperty)
            {
                SysAni.Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Children[3].X"));
                //SetProp("RenderTransform.Children[2].X", animation, copy);
            }
            else if (prop == TranslateTransform.YProperty)
            {
                SysAni.Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.Children[3].Y"));
                //SetProp("RenderTransform.Children[2].Y", animation, copy);
            }
            else if (prop == ScaleTransform.ScaleXProperty)
            {
                SysAni.Storyboard.SetTargetProperty(animation,
                    new PropertyPath("RenderTransform.Children[0].ScaleX"));
                //SetProp("RenderTransform.Children[0].ScaleX", animation, copy);
            }
            else if (prop == ScaleTransform.ScaleYProperty)
            {
                SysAni.Storyboard.SetTargetProperty(animation,
                    new PropertyPath("RenderTransform.Children[0].ScaleY"));
                //SetProp("RenderTransform.Children[0].ScaleY", animation, copy);
            }
            else if (prop == SkewTransform.AngleXProperty)
            {
                SysAni.Storyboard.SetTargetProperty(animation,
                    new PropertyPath("RenderTransform.ScaleX"));
                //SetProp("RenderTransform.Children[0].ScaleX", animation, copy);
            }
            else if (prop == SkewTransform.AngleYProperty)
            {
                SysAni.Storyboard.SetTargetProperty(animation,
                    new PropertyPath("RenderTransform.ScaleY"));
                //SetProp("RenderTransform.Children[0].ScaleY", animation, copy);
            }
            else if (prop == SolidColorBrush.ColorProperty)
            {
                SysAni.Storyboard.SetTarget(animation, _image);
                SysAni.Storyboard.SetTargetProperty(animation,
                    new PropertyPath("Effect.Blend.Visual.Drawing.Children[0].Brush.Color"));
                //SetProp("RenderTransform.Children[0].ScaleY", animation, copy);
            }
            else
            {
                SetProp(prop, animation, copy);
            }
        }
    }

    private static void SetProp(DependencyProperty path, params AnimationTimeline[] timelines)
    {
        foreach (var timeline in timelines)
        {
            if (timeline != null)
                SysAni.Storyboard.SetTargetProperty(timeline, new PropertyPath(path));
        }
    }

    private static void SetProp(string path, params AnimationTimeline[] timelines)
    {
        foreach (var timeline in timelines)
        {
            if (timeline != null)
                SysAni.Storyboard.SetTargetProperty(timeline, new PropertyPath(path));
        }
    }

    private IEasingFunction ConvertEasing(Easing easing)
    {
        switch (easing)
        {
            case Easing.Linear:
                return null;
            case Easing.SineOut:
            case Easing.EasingOut:
                return new SineEase { EasingMode = EasingMode.EaseOut };
            case Easing.SineIn:
            case Easing.EasingIn:
                return new SineEase { EasingMode = EasingMode.EaseIn };
            case Easing.SineInOut:
                return new SineEase { EasingMode = EasingMode.EaseInOut };
            case Easing.QuadIn:
                return new QuadraticEase { EasingMode = EasingMode.EaseIn };
            case Easing.QuadOut:
                return new QuadraticEase { EasingMode = EasingMode.EaseOut };
            case Easing.QuadInOut:
                return new QuadraticEase { EasingMode = EasingMode.EaseInOut };
            case Easing.CubicIn:
                return new CubicEase { EasingMode = EasingMode.EaseIn };
            case Easing.CubicOut:
                return new CubicEase { EasingMode = EasingMode.EaseOut };
            case Easing.CubicInOut:
                return new CubicEase { EasingMode = EasingMode.EaseInOut };
            case Easing.QuartIn:
                return new QuarticEase { EasingMode = EasingMode.EaseIn };
            case Easing.QuartOut:
                return new QuarticEase { EasingMode = EasingMode.EaseOut };
            case Easing.QuartInOut:
                return new QuarticEase { EasingMode = EasingMode.EaseInOut };
            case Easing.QuintIn:
                return new QuinticEase { EasingMode = EasingMode.EaseIn };
            case Easing.QuintOut:
                return new QuinticEase { EasingMode = EasingMode.EaseOut };
            case Easing.QuintInOut:
                return new QuinticEase { EasingMode = EasingMode.EaseInOut };
            case Easing.ExpoIn:
                return new ExponentialEase { EasingMode = EasingMode.EaseIn };
            case Easing.ExpoOut:
                return new ExponentialEase { EasingMode = EasingMode.EaseOut };
            case Easing.ExpoInOut:
                return new ExponentialEase { EasingMode = EasingMode.EaseInOut };
            case Easing.CircIn:
                return new CircleEase { EasingMode = EasingMode.EaseIn };
            case Easing.CircOut:
                return new CircleEase { EasingMode = EasingMode.EaseOut };
            case Easing.CircInOut:
                return new CircleEase { EasingMode = EasingMode.EaseInOut };
            case Easing.ElasticIn:
                return new ElasticEase { EasingMode = EasingMode.EaseIn };
            case Easing.ElasticOut:
                return new ElasticEase { EasingMode = EasingMode.EaseOut };
            case Easing.ElasticHalfOut:
                return new ElasticEase { EasingMode = EasingMode.EaseInOut, Springiness = 1.5 };
            case Easing.ElasticQuarterOut:
                return new ElasticEase { EasingMode = EasingMode.EaseInOut, Springiness = 1.5 };
            case Easing.ElasticInOut:
                return new ElasticEase { EasingMode = EasingMode.EaseInOut };
            case Easing.BackIn:
                return new BackEase { EasingMode = EasingMode.EaseIn };
            case Easing.BackOut:
                return new BackEase { EasingMode = EasingMode.EaseOut };
            case Easing.BackInOut:
                return new BackEase { EasingMode = EasingMode.EaseInOut };
            case Easing.BounceIn:
                return new BounceEase { EasingMode = EasingMode.EaseIn };
            case Easing.BounceOut:
                return new BounceEase { EasingMode = EasingMode.EaseOut };
            case Easing.BounceInOut:
                return new BounceEase { EasingMode = EasingMode.EaseInOut };
            default:
                throw new ArgumentOutOfRangeException(nameof(easing), easing, null);
        }

        //return null;
        throw new NotSupportedException($"不支持{easing}", null);
    }

    public void AddToCanvas()
    {
        if (_blendStatus == AnimateStatus.None)
        {
            Host?.Canvas.Children.Add(_image);
        }
        else if (_blendStatus == AnimateStatus.Static)
        {
            Host?.Canvas.Children.Add(_addImage);
        }
        else
        {
            Host?.Canvas.Children.Add(_image);
            Host?.Canvas.Children.Add(_addImage);
        }
    }

    public void ClearObj()
    {
        if (Storyboard.FillBehavior != FillBehavior.Stop)
        {
            Storyboard.Stop();
            if (_blendStatus == AnimateStatus.None)
            {
                Host?.Canvas.Children.Remove(_image);
            }
            else if (_blendStatus == AnimateStatus.Static)
            {
                Host?.Canvas.Children.Remove(_addImage);
            }
            else
            {
                Host?.Canvas.Children.Remove(_image);
                Host?.Canvas.Children.Remove(_addImage);
            }
        }
    }
}

public class AComparer : IComparer<(ImageObject, double)>
{
    public int Compare((ImageObject, double) x, (ImageObject, double) y)
    {
        return x.Item2.CompareTo(y.Item2);
    }
}

public enum AnimateStatus
{
    None,
    Static,
    Dynamic
}