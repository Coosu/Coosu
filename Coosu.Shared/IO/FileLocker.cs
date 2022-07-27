using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Coosu.Shared.IO
{
    public class FileLocker : IDisposable
    {
        private readonly string _path;
        private readonly string _lockPath;
        private readonly FileStream? _fs;

        public FileLocker(string path)
        {
            _path = path;
            _lockPath = Path.Combine(Path.GetDirectoryName(path) ?? ".", Path.GetFileName(path) + ".lock");
            var fi = new FileInfo(_lockPath);
            if (fi.Exists && DateTime.Now - fi.CreationTime > TimeSpan.FromSeconds(100))
            {
                fi.Delete();
                //throw new Exception("Failed: waiting for too long. Please check the .lock file.");
            }

            bool success = false;
            var sw = Stopwatch.StartNew();
            bool notified = false;
            while (!success)
            {
                if (sw.Elapsed >= TimeSpan.FromSeconds(30))
                    throw new Exception("Failed: waiting for too long. Please check the .lock file.");
                try
                {
                    _fs = new FileStream(_lockPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
                    Console.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId}: Get the lock");
                    success = true;
                }
                catch
                {
                    if (!notified) Console.WriteLine($"Thread #{Thread.CurrentThread.ManagedThreadId}: Waiting for lock");
                    notified = true;
                    Thread.Sleep(1);
                    // ignored
                }
            }
        }

        public void Dispose()
        {
            _fs?.Dispose();
            System.IO.File.Delete(_lockPath);
        }
    }
}