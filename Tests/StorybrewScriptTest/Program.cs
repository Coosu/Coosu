using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Coosu.Shared;
using StorybrewCommon.Mapset;
using StorybrewCommon.Storyboarding;
using StorybrewEditor.Storyboarding;
using Vector2 = OpenTK.Vector2;

namespace StorybrewScriptTest
{

    class Program
    {

        static void Main(string[] args)
        {
            var cls = new Class();
            cls.Generate(new MyContext());
        }
    }

    internal class MyStoryboardLayer : StoryboardLayer
    {
        public MyStoryboardLayer(string identifier) : base(identifier)
        {
        }

        public override OsbSprite CreateSprite(string path, OsbOrigin origin, Vector2 initialPosition)
        {
            var storyboardObject = new EditorOsbSprite()
            {
                TexturePath = path,
                Origin = origin,
                InitialPosition = initialPosition,
            };
            return storyboardObject;
        }

        public override OsbSprite CreateSprite(string path, OsbOrigin origin)
            => CreateSprite(path, origin, OsbSprite.DefaultPosition);

        public override OsbAnimation CreateAnimation(string path, int frameCount, double frameDelay, OsbLoopType loopType, OsbOrigin origin,
            Vector2 initialPosition)
        {
            var storyboardObject = new EditorOsbAnimation()
            {
                TexturePath = path,
                Origin = origin,
                FrameCount = frameCount,
                FrameDelay = frameDelay,
                LoopType = loopType,
                InitialPosition = initialPosition,
            };
            return storyboardObject;
        }

        public override OsbAnimation CreateAnimation(string path, int frameCount, double frameDelay, OsbLoopType loopType, OsbOrigin origin = OsbOrigin.Centre)
            => CreateAnimation(path, frameCount, frameDelay, loopType, origin, OsbSprite.DefaultPosition);

        public override OsbSample CreateSample(string path, double time, double volume = 100)
        {
            var storyboardObject = new EditorOsbSample()
            {
                AudioPath = path,
                Time = time,
                Volume = volume,
            };
            return storyboardObject;
        }
    }

    internal class MyContext : GeneratorContext
    {
        public override string ProjectPath { get; } = "./demo-project";
        public override string ProjectAssetPath { get; } = "./demo-project/Assert";
        public override string MapsetPath { get; } = "./demo-mapset";
        public override void AddDependency(string path)
        {
        }

        public override void AppendLog(string message)
        {
            Console.WriteLine(message);
        }

        public override Beatmap Beatmap { get; }
        public override IEnumerable<Beatmap> Beatmaps { get; } = EmptyArray<Beatmap>.Value;
        public override StoryboardLayer GetLayer(string identifier)
        {
            return new MyStoryboardLayer(identifier);
        }

        public override double AudioDuration { get; } = 114514;
        public override float[] GetFft(double time, string path = null)
        {
            return EmptyArray<float>.Value;
        }
    }
}