using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Coosu.Storyboard.Storybrew.Text;

namespace TextWpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Grid.Children.Add(new TextControl(new TextContext()
            {
                Text = "🥰🥰 LYRIC lyric here... owo!~|\"/\\á测试<>".ToCharArray(),
                TextOptions = new CoosuTextOptionsBuilder()
                    .WithIdentifier("style1")
                    .WithFontFamily("Consolas")
                    .WithFontFamily("Simsun")
                    .WithFontSize(48)
                    .WithWordGap(5)
                    .WithLineGap(10)
                    .ScaleXBy(0.7)
                    .Reverse()
                    .FillBy("#43221451")
                    .FillLinearGradientBy("#43221451", "#000000", 60)
                    .WithStroke("#FF131451", 5)
                    .WithShadow("#000000", 10, -60, 4)
                    .Options
            }));
        }
    }
}
