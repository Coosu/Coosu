using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Coosu.Storyboard.Advanced.Text
{
    public class CoosuTextOptions
    {
        internal bool IsInitialFamily = true;
        public string? StrokeBrushJson { get; private set; }
        public string FillBrushJson { get; private set; } = JsonConvert.SerializeObject(Brushes.White, new BrushJsonConverter());
        private Brush _fillBrush;
        private Brush? _strokeBrush;
        public string? FileIdentifier { get; set; }
        public bool RightToLeft { get; set; } = false;
        public double XScale { get; set; } = 1;
        public double YScale { get; set; } = 1;
        public double WordGap { get; set; }
        public double LineGap { get; set; }
        public Orientation Orientation { get; set; } = Orientation.Horizontal;
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
        public double ShadowDirection { get; set; } = -45;
        public double ShadowDepth { get; set; } = 5;

        internal static CoosuTextOptions Default { get; } = new()
        {
            FileIdentifier = "default"
        };

        public string GetBaseId()
        {
            var availableObj = new
            {
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
}