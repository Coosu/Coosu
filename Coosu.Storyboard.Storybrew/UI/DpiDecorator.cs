using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Coosu.Storyboard.Storybrew.UI;

public class DpiDecorator : Decorator
{
    public DpiDecorator()
    {
        this.Loaded += (s, e) =>
        {
            Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            ScaleTransform dpiTransform = new ScaleTransform(1 / m.M11, 1 / m.M22);
            if (dpiTransform.CanFreeze)
                dpiTransform.Freeze();
            this.LayoutTransform = dpiTransform;
        };
    }
};