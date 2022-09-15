using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coosu.Shared.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StorybrewCommon.Scripting;
using File = System.IO.File;

// ReSharper disable once CheckNamespace
namespace Coosu.Storyboard;

public static class StoryboardObjectGeneratorExtensions
{
    public static string GetProjectCachePath(this StoryboardObjectGenerator brewObjectGenerator)
    {
        return GetProjectPath(brewObjectGenerator, "cache");
    }

    public static string GetProjectConfigPath(this StoryboardObjectGenerator brewObjectGenerator)
    {
        return GetProjectPath(brewObjectGenerator, "config");
    }

    public static T GetStore<T>(this StoryboardObjectGenerator generator, Func<JToken, T> property)
    {
        var path = generator.GetProjectConfigPath();
        using (new FileLocker(path))
        {
            if (!File.Exists(path)) File.WriteAllText(path, "{}");
            var text = File.ReadAllText(path);
            var root = JToken.Parse(text);

            return property.Invoke(root);
        }
    }

    public static void SetStore(this StoryboardObjectGenerator generator, Action<JToken> configuration)
    {
        var path = generator.GetProjectConfigPath();
        using (new FileLocker(path))
        {
            if (!File.Exists(path)) File.WriteAllText(path, "{}");
            var text = File.ReadAllText(path);
            var root = JToken.Parse(text);

            configuration.Invoke(root);
            var save = root.ToString(Formatting.Indented);
            File.WriteAllText(path, save);
        }
    }

    private static string GetProjectPath(StoryboardObjectGenerator brewObjectGenerator, string name)
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

        var configFile = Path.Combine(targetProjectDir, name + ".json");
        return configFile;
    }
}