using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Coosu.Storyboard.Storybrew.Text;

public class CoosuTextOptions
{
    internal bool IsInitialFamily = true;
    private Brush _fillBrush = Brushes.White;
    private Brush? _strokeBrush;

    public string? StrokeBrushJson { get; private set; }
    public string FillBrushJson { get; private set; } = JsonConvert.SerializeObject(Brushes.White, new BrushJsonConverter());

    public string? FileIdentifier { get; set; }
    public bool RightToLeft { get; set; } = false;
    public double XScale { get; set; } = 1;
    public double YScale { get; set; } = 1;
    public double WordGap { get; set; }
    public double LineGap { get; set; }
    public OriginType Origin { get; set; } = OriginType.Centre;
    public Orientation Orientation { get; set; } = Orientation.Horizontal;
    public bool RotateBy90 { get; set; }
    //fsnm;fsob;fsit
    public FontStyle FontStyle { get; set; } = FontStyles.Normal;
    //fw400
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
    //s36
    public int FontSize { get; set; } = 36;
    public List<FontFamilySource> FontFamilies { get; set; } = new() { "Arial" };

    public Brush FillBrush
    {
        get => _fillBrush;
        set
        {
            _fillBrush = value;
            FillBrushJson = JsonConvert.SerializeObject(value, new BrushJsonConverter());
        }
    }

    //ston;st
    public OptionType StrokeMode { get; set; }

    public Brush? StrokeBrush
    {
        get { return _strokeBrush; }
        set
        {
            StrokeBrushJson = JsonConvert.SerializeObject(value, new BrushJsonConverter());
            _strokeBrush = value;
        }
    }

    public double StrokeThickness { get; set; }
    //sdon;sd
    public OptionType ShadowMode { get; set; }
    public Color? ShadowColor { get; set; }
    public double ShadowBlurRadius { get; set; } = 5;
    public double ShadowDirection { get; set; } = 45;
    public double ShadowDepth { get; set; } = 5;

    public bool ShowBase => StrokeMode != OptionType.Only &&
                            ShadowMode != OptionType.Only;
    public bool ShowStroke => StrokeMode != OptionType.None &&
                              ShadowMode != OptionType.Only;
    public bool ShowShadow => ShadowMode != OptionType.None;

    internal static CoosuTextOptions Default { get; } = new()
    {
        FileIdentifier = "default"
    };

    public string GetBaseId()
    {
        var availableObj = new
        {
            ShowBase,
            FontStyle,
            FontWeight,
            FontSize,
            FontFamilies,
            FillBrushJson,
        };
        return JsonConvert.SerializeObject(availableObj);
    }

    public string GetStrokeId()
    {
        var availableObj = new
        {
            ShowStroke,
            FontStyle,
            FontWeight,
            FontSize,
            FontFamilies,
            StrokeBrushJson,
            StrokeThickness,
        };
        return JsonConvert.SerializeObject(availableObj);
    }

    public string GetShadowId()
    {
        var availableObj = new
        {
            ShowShadow,
            FontStyle,
            FontWeight,
            FontSize,
            FontFamilies,
            ShadowColor,
            ShadowBlurRadius,
            ShadowDirection,
            ShadowDepth,
        };
        return JsonConvert.SerializeObject(availableObj);
    }
}