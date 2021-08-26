using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StorybrewCommon.Scripting;
using Tiny;
using Tiny.Formats.Yaml;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard
{
    public static class StoryboardObjectGeneratorExtensions
    {

        public static string GetProjectConfigPath(this StoryboardObjectGenerator brewObjectGenerator)
        {
            var brewPath = AppDomain.CurrentDomain.BaseDirectory;
            var coosuPath = Path.Combine(brewPath, ".coosu");
            var coosuProjectDir = Path.Combine(coosuPath, "projects");

            var projectPath = brewObjectGenerator.ProjectPath;
            var projectName = Path.GetFileName(projectPath);

            var targetProjectDir = Path.Combine(coosuProjectDir, projectName);
            try
            {
                Directory.CreateDirectory(targetProjectDir);
            }
            catch
            {
                // ignored
            }

            var configFile = Path.Combine(targetProjectDir, "config.yaml");
            return configFile;
        }

        public static T GetStore<T>(this StoryboardObjectGenerator generator, Func<TinyToken, T> property)
        {
            var path = generator.GetProjectConfigPath();
            using (new FileLocker(path))
            {
                if (!File.Exists(path)) File.WriteAllText(path, "");

                var root = TinyToken.Read(path);

                return property.Invoke(root);
            }
        }

        public static void SetStore(this StoryboardObjectGenerator generator, Action<TinyToken> configuration)
        {
            var path = generator.GetProjectConfigPath();
            using (new FileLocker(path))
            {
                var root = TinyToken.Read(path);

                configuration.Invoke(root);
                root.Write(path);
            }
        }
    }

    public class FileLocker : IDisposable
    {
        private readonly string _path;
        private readonly string _lockPath;

        public FileLocker(string path)
        {
            _path = path;
            _lockPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path) + ".lock");
            var fi = new FileInfo(_lockPath);
            if (fi.Exists && DateTime.Now - fi.CreationTime > TimeSpan.FromSeconds(30))
            {
                throw new Exception("Failed: waiting for too long. Please check the .lock file.");
            }

            bool success = false;
            var sw = Stopwatch.StartNew();
            while (!success)
            {
                if (sw.Elapsed >= TimeSpan.FromSeconds(30))
                    throw new Exception("Failed: waiting for too long. Please check the .lock file.");
                try
                {
                    using var fs = new FileStream(_lockPath, FileMode.CreateNew);
                    success = true;
                }
                catch
                {
                    Thread.Sleep(1);
                    // ignored
                }
            }
        }

        public void Dispose()
        {
            File.Delete(_lockPath);
        }
    }
}
