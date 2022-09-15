using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Coosu.Storyboard.Storybrew.UI;

public static class UiThreadHelper
{
    private static Thread? _uiThread;
    internal static Application? Application;
    private static readonly ReaderWriterLockSlim UiThreadCheckLock = new();
    private static readonly TaskCompletionSource<bool> WaitComplete = new();

    public static void Shutdown()
    {
        Application?.Dispatcher.Invoke(() => Application?.Shutdown());
    }

    internal static void EnsureUiThreadAlive()
    {
        try
        {
            UiThreadCheckLock.EnterReadLock();
            if (_uiThread is { IsAlive: true })
            {
                return;
            }
        }
        finally
        {
            UiThreadCheckLock.ExitReadLock();
        }

        UiThreadCheckLock.EnterWriteLock();
        _uiThread = new Thread(() =>
        {
            Application = new Application
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };

            Application.Startup += (_, __) => WaitComplete.SetResult(true);
            try
            {
                Application.Run();
            }
            catch (Exception ex)
            {
                Application.Shutdown();
            }
        })
        {
            IsBackground = true
        };
        _uiThread.SetApartmentState(ApartmentState.STA);
        _uiThread.Start();
        WaitComplete.Task.Wait();
        UiThreadCheckLock.ExitWriteLock();
    }
}