using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Coosu.Storyboard.Advanced.Text
{
    public class TextControlVm : INotifyPropertyChanged
    {
        private string? _text = "milkitic";
        private IList<double>? _widths;
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
        private FontFamily _fontFamily =
            (FontFamily)new FontFamilyConverter().ConvertFrom("D:/GitHub/98.xaml/ControlTest/Resources/simsun.ttf#simsun");

        private double _shadowOpacity = 0.6;
        //(FontFamily)new FontFamilyConverter().ConvertFrom("arial,simsun");

        public string? Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        public IList<double>? Widths
        {
            get => _widths;
            set
            {
                if (Equals(value, _widths)) return;
                _widths = value;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Interaction logic for TextControl.xaml
    /// </summary>
    public partial class TextControl : UserControl
    {
        TextControlVm _viewModel;
        public TextControl()
        {
            InitializeComponent();
            DataContext = _viewModel = new TextControlVm();
            Loaded += MainWindow_Loaded;
            Compute();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.Text = "qwrtyi'm@%^&*+中 \n";
            Compute();
        }

        private async void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(16);
            Compute();
        }

        private void Compute()
        {
            var dic = new Dictionary<char, double>();
            var list = new List<double>();
            foreach (var obj in ComputingStandardControl.Items)
            {
                var c = (char)obj;
                if (dic.TryGetValue(c, out var val))
                {
                    list.Add(val);
                }
                else
                {
                    var cp = (ContentPresenter)ComputingStandardControl.ItemContainerGenerator.ContainerFromItem(c);
                    var width = cp.ActualWidth - _viewModel.ShadowBlurRadius * 2 - _viewModel.StrokeThickness * 2;
                    width = Math.Round(width, 4);
                    if (!dic.ContainsKey(c))
                        dic.Add(c, width);
                    list.Add(width);
                }
            }

            _viewModel.Widths = list;
        }


        public static T? FindVisualChild<T>(DependencyObject? depObj) where T : DependencyObject
        {
            if (depObj == null) return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T dependencyObject)
                {
                    return dependencyObject;
                }

                var childItem = FindVisualChild<T>(child);
                if (childItem != null) return childItem;
            }

            return null;
        }
    }
}
