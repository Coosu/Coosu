using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coosu.Shared.IO;
using Coosu.Shared.Numerics;
using Newtonsoft.Json;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using File = System.IO.File;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;
using Size = System.Windows.Size;

namespace Coosu.Storyboard.Storybrew.Text;

public class TextControlVm : INotifyPropertyChanged
{
    private IList<char>? _text = "milkitic".Distinct().ToArray();
    private IList<Vector2D>? _sizes;
    private FontStyle _fontStyle = FontStyles.Normal;
    private FontWeight _fontWeight = FontWeights.Normal;
    private int _fontSize = 36;
    private Brush _fillBrush = Brushes.White;
    private Brush? _strokeBrush = Brushes.Orange;
    private double _strokeThickness = 2;
    private Color? _shadowColor = Colors.Aqua;
    private double _shadowBlurRadius = 5;
    private double _shadowDirection = -45;
    private double _shadowDepth = 5;
    private FontFamily _fontFamily = (FontFamily)new FontFamilyConverter()
        .ConvertFrom("D:/GitHub/98.xaml/ControlTest/Resources/simsun.ttf#simsun");

    private double _shadowOpacity = 0.6;
    //(FontFamily)new FontFamilyConverter().ConvertFrom("arial,simsun");

    public IList<char>? Text
    {
        get => _text;
        set
        {
            if (value == _text) return;
            _text = value;
            OnPropertyChanged();
        }
    }

    public FontStyle FontStyle
    {
        get => _fontStyle;
        set
        {
            if (value == _fontStyle) return;
            _fontStyle = value;
            OnPropertyChanged();
        }
    }

    public FontWeight FontWeight
    {
        get => _fontWeight;
        set
        {
            if (value.Equals(_fontWeight)) return;
            _fontWeight = value;
            OnPropertyChanged();
        }
    }

    public int FontSize
    {
        get => _fontSize;
        set
        {
            if (value == _fontSize) return;
            _fontSize = value;
            OnPropertyChanged();
        }
    }

    public Brush FillBrush
    {
        get => _fillBrush;
        set
        {
            if (Equals(value, _fillBrush)) return;
            _fillBrush = value;
            OnPropertyChanged();
        }
    }

    public Brush? StrokeBrush
    {
        get => _strokeBrush;
        set
        {
            if (Equals(value, _strokeBrush)) return;
            _strokeBrush = value;
            OnPropertyChanged();
        }
    }

    public double StrokeThickness
    {
        get => _strokeThickness;
        set
        {
            if (value.Equals(_strokeThickness)) return;
            _strokeThickness = value;
            OnPropertyChanged();
        }
    }

    public Color? ShadowColor
    {
        get => _shadowColor;
        set
        {
            if (Equals(value, _shadowColor)) return;
            _shadowColor = value;
            OnPropertyChanged();
        }
    }

    public double ShadowBlurRadius
    {
        get => _shadowBlurRadius;
        set
        {
            if (value.Equals(_shadowBlurRadius)) return;
            _shadowBlurRadius = value;
            OnPropertyChanged();
        }
    }

    public double ShadowDirection
    {
        get => _shadowDirection;
        set
        {
            if (value.Equals(_shadowDirection)) return;
            _shadowDirection = value;
            OnPropertyChanged();
        }
    }

    public double ShadowDepth
    {
        get => _shadowDepth;
        set
        {
            if (value.Equals(_shadowDepth)) return;
            _shadowDepth = value;
            OnPropertyChanged();
        }
    }

    public double ShadowOpacity
    {
        get => _shadowOpacity;
        set
        {
            if (value.Equals(_shadowOpacity)) return;
            _shadowOpacity = value;
            OnPropertyChanged();
        }
    }

    public FontFamily FontFamily
    {
        get => _fontFamily;
        set
        {
            if (Equals(value, _fontFamily)) return;
            _fontFamily = value;
            OnPropertyChanged();
        }
    }

    public IList<Vector2D>? Sizes
    {
        get => _sizes;
        set
        {
            if (Equals(value, _sizes)) return;
            _sizes = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Interaction logic for TextControl.xaml
/// </summary>
public partial class TextControl : UserControl
{
    private readonly TextContext _textContext;
    private readonly TextControlVm _viewModel = new();
    private readonly bool _showBase;
    private readonly bool _showStroke;
    private readonly bool _showShadow;
    private Dictionary<char, Vector2D> _sizeDict = new();

    public TextControl(TextContext textContext)
    {
        InitializeComponent();

        _textContext = textContext;
        _viewModel.Text = textContext.Text.Distinct().ToArray();
        var textOptions = textContext.TextOptions;
        _viewModel.FontStyle = textOptions.FontStyle;
        _viewModel.FontWeight = textOptions.FontWeight;
        _viewModel.FontSize = textOptions.FontSize;
        _viewModel.FillBrush =
            JsonConvert.DeserializeObject<Brush>(textOptions.FillBrushJson, new BrushJsonConverter());
        if (textOptions.StrokeBrushJson != null)
            _viewModel.StrokeBrush =
                JsonConvert.DeserializeObject<Brush>(textOptions.StrokeBrushJson, new BrushJsonConverter());
        _viewModel.StrokeThickness = textOptions.StrokeThickness;
        if (textOptions.ShadowColor != null)
        {
            var color = textOptions.ShadowColor.Value;
            _viewModel.ShadowColor = Color.FromRgb(color.R, color.G, color.B);
            _viewModel.ShadowOpacity = color.A / 255d;
        }

        _viewModel.ShadowBlurRadius = textOptions.ShadowBlurRadius;
        _viewModel.ShadowDirection = textOptions.ShadowDirection;
        _viewModel.ShadowDepth = textOptions.ShadowDepth;
        var sb = new StringBuilder();
        for (var i = 0; i < textOptions.FontFamilies.Count; i++)
        {
            if (i != 0) sb.Append(',');
            var fontFamilySource = textOptions.FontFamilies[i];
            if (fontFamilySource.Path != null)
            {
                sb.Append(fontFamilySource.Path.Replace('\\', '/'));
                sb.Append('#');
            }

            sb.Append(fontFamilySource.Name);
        }

        _viewModel.FontFamily = (FontFamily)new FontFamilyConverter().ConvertFrom(sb.ToString());
        _showBase = textOptions.ShowBase;
        _showStroke = textOptions.ShowStroke;
        _showShadow = textOptions.ShowShadow;

        if (!_showBase) ComputingBaseControl.ItemsSource = null;
        if (!_showStroke) StrokeOnlyControl.ItemsSource = null;
        if (!_showShadow) ShadowOnlyControl.ItemsSource = null;

        DataContext = _viewModel;
    }

    private async void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        await Task.Delay(16);
        Compute();
    }

    private void Compute()
    {
        var dic = new Dictionary<char, Vector2D>();
        var list = new List<Vector2D>();
        foreach (var obj in ComputingBaseControl.Items)
        {
            var c = (char)obj!;
            if (dic.TryGetValue(c, out var val))
            {
                list.Add(val);
            }
            else
            {
                var cp = (ContentPresenter)ComputingBaseControl.ItemContainerGenerator.ContainerFromItem(c);
                var width = cp.ActualWidth - _viewModel.ShadowBlurRadius * 2 - _viewModel.StrokeThickness * 2;
                var height = cp.ActualHeight - _viewModel.ShadowBlurRadius * 2 - _viewModel.StrokeThickness * 2;
                width = Math.Round(width, 4);
                height = Math.Round(height, 4);
                var vector2 = new Vector2D(width, height);
                if (!dic.ContainsKey(c)) dic.Add(c, vector2);
                list.Add(vector2);
            }
        }

        _viewModel.Sizes = list;
        _sizeDict = dic;
    }


    public Dictionary<char, Vector2D> SaveImageAndGetWidth()
    {
        Compute();

        using var fileLocker = new FileLocker(_textContext.CachePath);
        if (!File.Exists(_textContext.CachePath))
            File.WriteAllText(_textContext.CachePath, "{}");
        var cache = JsonConvert.DeserializeObject<CacheObj>(File.ReadAllText(_textContext.CachePath))!;
        var baseId = _textContext.TextOptions.GetBaseId();
        var strokeId = _textContext.TextOptions.GetStrokeId();
        var shadowId = _textContext.TextOptions.GetShadowId();

        bool forceReBase = false, forceReStroke = false, forceReShadow = false;

        if (!cache.FontIdentifier.TryGetValue(_textContext.TextOptions.FileIdentifier!, out var fontTypeObj) ||
            fontTypeObj == null)
        {
            forceReBase = true;
            forceReStroke = true;
            forceReShadow = true;
        }
        else
        {
            if (fontTypeObj.Base != baseId) forceReBase = true;
            if (fontTypeObj.Stroke != strokeId) forceReStroke = true;
            if (fontTypeObj.Shadow != shadowId) forceReShadow = true;
        }

        Regenerate(forceReBase, forceReStroke, forceReShadow);

        if (forceReBase || forceReStroke || forceReShadow)
        {
            if (!cache.FontIdentifier.ContainsKey(_textContext.TextOptions.FileIdentifier!)||
                cache.FontIdentifier[_textContext.TextOptions.FileIdentifier!] == null)
                cache.FontIdentifier[_textContext.TextOptions.FileIdentifier!] = new FontTypeObj
                {
                    Base = baseId,
                    Shadow = shadowId,
                    Stroke = strokeId,
                    SizeMapping = _sizeDict
                };
            else
            {
                var obj = cache.FontIdentifier[_textContext.TextOptions.FileIdentifier!];
                obj.Base = baseId;
                obj.Shadow = shadowId;
                obj.Stroke = strokeId;
                if (obj.SizeMapping == null) obj.SizeMapping = _sizeDict;
                else
                {
                    foreach (var kvp in _sizeDict)
                    {
                        obj.SizeMapping[kvp.Key] = kvp.Value;
                    }
                }
            }
        }
        else
        {
            var obj = cache.FontIdentifier[_textContext.TextOptions.FileIdentifier!];
            if (obj.SizeMapping == null) obj.SizeMapping = _sizeDict;
            else
            {
                foreach (var kvp in _sizeDict)
                {
                    obj.SizeMapping[kvp.Key] = kvp.Value;
                }
            }
        }

        File.WriteAllText(_textContext.CachePath, JsonConvert.SerializeObject(cache, Formatting.Indented));

        return _sizeDict;
    }

    private void Regenerate(bool forceReBase, bool forceReStroke, bool forceReShadow)
    {
        var coosuTextDir = Path.Combine(_textContext.BeatmapsetDir, Directories.CoosuTextDir);
        if (!Directory.Exists(coosuTextDir))
            Directory.CreateDirectory(coosuTextDir);
        var prefix = _textContext.TextOptions.FileIdentifier + "_";
        if (_showBase)
        {
            var borders = FindVisualChildren<ContentControl>(ComputingBaseControl).ToArray();
            Save(borders, prefix, "", coosuTextDir, forceReBase);
        }

        if (_showStroke)
        {
            var borders = FindVisualChildren<ContentControl>(StrokeOnlyControl).ToArray();
            Save(borders, prefix, "_st", coosuTextDir, forceReStroke);
        }

        if (_showShadow)
        {
            var borders = FindVisualChildren<ContentControl>(ShadowOnlyControl).ToArray();
            Save(borders, prefix, "_bl", coosuTextDir, forceReShadow);
        }
    }

    private void Save(IEnumerable<ContentControl> contentControls, string prefix, string postFix, string coosuTextDir, bool force)
    {
        foreach (var contentControl in contentControls)
        {
            var target = (FrameworkElement)contentControl.Content;
            var c = (char)target.Tag;
            if (_sizeDict[c].X <= 0 || _sizeDict[c].Y <= 0) continue;
            if (c == ' ') continue;
            var fileName = TextHelper.ConvertToFileName(c, prefix, postFix);
            var filePath = Path.Combine(coosuTextDir, fileName);

            if (File.Exists(filePath) && !force) continue;
            var image = GetImageByVisual(new Size(contentControl.ActualWidth + _viewModel.StrokeThickness * 2,
                contentControl.ActualHeight + _viewModel.StrokeThickness * 2), contentControl);
            image.Save(filePath);
            image.Dispose();
        }
    }
    private static System.Drawing.Image GetImageByVisual(Size size, Visual fe)
    {
        var dpi = 96;
        var bitmap = new RenderTargetBitmap(
            (int)(size.Width * dpi / 96), (int)(size.Height * dpi / 96),
            dpi, dpi, PixelFormats.Pbgra32
        );

        bitmap.Render(fe);
        using var stream = new MemoryStream();
        BitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        encoder.Save(stream);
        var bitmap1 = new System.Drawing.Bitmap(stream);
        return bitmap1;
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject? depObj) where T : DependencyObject
    {
        if (depObj == null) yield break;
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
            if (child is T dependencyObject)
            {
                yield return dependencyObject;
            }

            foreach (T childOfChild in FindVisualChildren<T>(child))
            {
                yield return childOfChild;
            }
        }
    }
}