using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Coosu.Storyboard.Storybrew.UI;

public class WindowBase : Window
{
    public WindowBase()
    {
        SizeToContent = SizeToContent.WidthAndHeight;
        Width = 0;
        Height = 0;
        AllowsTransparency = true;
        WindowStyle = WindowStyle.None;
        ShowInTaskbar = false;
        Focusable = false;
        Visibility = Visibility.Hidden;
        Opacity = 0;
        ShowActivated = false;
    }

    public bool IsShown { get; private set; }

    /// <summary>
    /// 窗体显示事件
    /// </summary>
    public static readonly RoutedEvent ShownEvent = EventManager.RegisterRoutedEvent
        ("Shown", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WindowBase));

    /// <summary>
    /// 当窗体显示时发生。
    /// </summary>
    public event RoutedEventHandler Shown
    {
        add => AddHandler(ShownEvent, value);
        remove => RemoveHandler(ShownEvent, value);
    }

    protected sealed override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);

        if (IsShown)
            return;

        IsShown = true;

        var args = new RoutedEventArgs(ShownEvent, this);
        RaiseEvent(args);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var source = PresentationSource.FromVisual(this) as HwndSource;
        source?.AddHook(WndProc);
        // Get this window's handle
        IntPtr hwnd = new WindowInteropHelper(this).Handle;

        // Change the extended window style to include WS_EX_TRANSPARENT
        int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
    }

    private const int WM_DPICHANGED = 0x02E0;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int GWL_EXSTYLE = (-20);

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_DPICHANGED)
        {
            handled = true;
        }
        return IntPtr.Zero;
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
}