﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Coosu.Beatmap.Extensions;
using Coosu.Beatmap.Extensions.Playback;
using Coosu.Beatmap.Sections.GamePlay;
using Coosu.Beatmap.Sections.HitObject;
using Coosu.Beatmap.Sections.Timing;

namespace Coosu.Beatmap
{
    public sealed class OsuDirectory
    {
        private readonly string _directory;
        private readonly HitsoundFileCache _cache = new();
        private bool _isInitialized;
        public IReadOnlyList<OsuFile> OsuFiles { get; private set; }
        public IReadOnlyList<string> WaveFiles { get; private set; }

        public OsuDirectory(string directory)
        {
            _directory = new DirectoryInfo(directory).FullName;
        }

        public async Task InitializeAsync()
        {
            var directoryInfo = new DirectoryInfo(_directory);
            var waveFiles = new List<string>();
            var osuFiles = new List<OsuFile>();
            await Task.Run(() => directoryInfo
                .EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                .AsParallel()
                .ForAll(k =>
                {
                    var ext = Path.GetExtension(k.Name);
                    if (Array.IndexOf(HitsoundFileCache.SupportExtensions, ext) != -1)
                    {
                        lock (waveFiles)
                        {
                            waveFiles.Add(k.Name);
                        }
                    }
                    else if (ext == ".osu")
                    {
                        lock (osuFiles)
                            osuFiles.Add(OsuFile.ReadFromFile(k.FullName));
                    }
                })
            );

            WaveFiles = waveFiles;
            OsuFiles = osuFiles;
            _isInitialized = true;
        }

        public async Task<List<HitsoundNode>> GetHitsoundNodesAsync(OsuFile osuFile)
        {
            if (_isInitialized == false) throw new Exception("The directory was not initialized");
            osuFile.HitObjects.ComputeSlidersByCurrentSettings();

            var hitObjects = osuFile.HitObjects.HitObjectList;
            var elements = new ConcurrentBag<HitsoundNode>();
            await Task.Run(() =>
            {
                hitObjects
                    .AsParallel()
                    .ForAll(obj =>
                    {
                        try
                        {
                            AddSingleHitObject(osuFile, obj, elements);
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error while analyzing the object: " + obj.ToSerializedString(), e);
                        }
                    });
            }).ConfigureAwait(false);

            return elements.ToList();
        }

        private void AddSingleHitObject(OsuFile osuFile, RawHitObject hitObject, ConcurrentBag<HitsoundNode> elements)
        {
            var ignoreBalance = osuFile.General.Mode == GameMode.Taiko;

            if (hitObject.ObjectType != HitObjectType.Slider)
            {
                var itemOffset = hitObject.ObjectType == HitObjectType.Spinner
                    ? hitObject.HoldEnd // spinner
                    : hitObject.Offset; // hold & circle
                var timingPoint = osuFile.TimingPoints.GetLine(itemOffset);

                float balance = ignoreBalance ? 0 : GetObjectBalance(hitObject.X);
                float volume = GetObjectVolume(hitObject, timingPoint);

                var tuples = AnalyzeHitsoundFiles(hitObject.Hitsound,
                    hitObject.SampleSet, hitObject.AdditionSet,
                    timingPoint, hitObject, osuFile);
                foreach (var (filename, useUserSkin, _) in tuples)
                {
                    var element = HitsoundNode.Create(itemOffset, volume, balance, filename, useUserSkin);
                    elements.Add(element);
                }
            }
            else // sliders
            {
                // edges
                bool forceUseSlide = hitObject.SliderInfo.EdgeSamples == null;
                var sliderEdges = hitObject.SliderInfo.GetEdges();
                foreach (var item in sliderEdges)
                {
                    var itemOffset = item.Offset;
                    var timingPoint = osuFile.TimingPoints.GetLine(itemOffset);

                    float balance = ignoreBalance ? 0 : GetObjectBalance(item.Point.X);
                    float volume = GetObjectVolume(hitObject, timingPoint);

                    var hitsoundType = forceUseSlide
                        ? hitObject.Hitsound
                        : item.EdgeHitsound;
                    var addition = forceUseSlide
                        ? hitObject.AdditionSet
                        : item.EdgeAddition;
                    var sample = forceUseSlide
                        ? hitObject.SampleSet
                        : item.EdgeSample;
                    var tuples = AnalyzeHitsoundFiles(hitsoundType,
                        sample, addition,
                        timingPoint, hitObject, osuFile);
                    foreach (var (filename, useUserSkin, _) in tuples)
                    {
                        var element = HitsoundNode.Create((int)itemOffset, volume, balance, filename, useUserSkin);
                        elements.Add(element);
                    }
                }

                // ticks
                var ticks = hitObject.SliderInfo.GetSliderTicks();
                foreach (var sliderTick in ticks)
                {
                    var itemOffset = sliderTick.Offset;
                    var timingPoint = osuFile.TimingPoints.GetLine(itemOffset);

                    float balance = ignoreBalance ? 0 : GetObjectBalance(sliderTick.Point.X);
                    float volume = GetObjectVolume(hitObject, timingPoint) * 1.25f; // ticks x1.25

                    var (filename, useUserSkin, _) = AnalyzeHitsoundFiles(HitsoundType.Tick,
                            hitObject.SampleSet, hitObject.AdditionSet,
                            timingPoint, hitObject, osuFile)
                        .First();

                    var element = HitsoundNode.Create((int)itemOffset, volume, balance, filename, useUserSkin);
                    elements.Add(element);
                }

                // sliding
                {
                    var slideElements = new List<HitsoundNode>();

                    var startOffset = hitObject.Offset;
                    var endOffset = sliderEdges[sliderEdges.Length - 1].Offset;
                    var timingPoint = osuFile.TimingPoints.GetLine(startOffset);

                    float balance = ignoreBalance ? 0 : GetObjectBalance(hitObject.X);
                    float volume = GetObjectVolume(hitObject, timingPoint);

                    // start sliding
                    var tuples = AnalyzeHitsoundFiles(
                        hitObject.Hitsound & HitsoundType.SlideWhistle | HitsoundType.Slide,
                        hitObject.SampleSet, hitObject.AdditionSet,
                        timingPoint, hitObject, osuFile);
                    foreach (var (filename, useUserSkin, hitsoundType) in tuples)
                    {
                        SlideChannel channel;
                        if (hitsoundType.HasFlag(HitsoundType.Slide))
                            channel = SlideChannel.Normal;
                        else if (hitsoundType.HasFlag(HitsoundType.SlideWhistle))
                            channel = SlideChannel.Whistle;
                        else
                            continue;

                        var element =
                            HitsoundNode.CreateLoopSignal(startOffset, volume, balance, filename, useUserSkin, channel);
                        slideElements.Add(element);
                    }

                    // change sample (will optimize if only adjust volume) by timing points
                    var timingsOnSlider = osuFile.TimingPoints.TimingList
                        .Where(k => k.Offset > startOffset + 0.5 && k.Offset < endOffset)
                        .ToList();

                    for (var i = 0; i < timingsOnSlider.Count; i++)
                    {
                        var timing = timingsOnSlider[i];
                        var prevTiming = i == 0 ? timingPoint : timingsOnSlider[i - 1];
                        if (timing.Track != prevTiming.Track ||
                            timing.TimingSampleset != prevTiming.TimingSampleset)
                        {
                            volume = GetObjectVolume(hitObject, timing);
                            tuples = AnalyzeHitsoundFiles(
                                hitObject.Hitsound & HitsoundType.SlideWhistle | HitsoundType.Slide,
                                hitObject.SampleSet, hitObject.AdditionSet,
                                timing, hitObject, osuFile);
                            foreach (var (filename, useUserSkin, hitsoundType) in tuples)
                            {
                                HitsoundNode element;
                                if (hitsoundType.HasFlag(HitsoundType.Slide) && slideElements
                                        .Last(k => k is ControlNode { ControlType: ControlType.StartSliding })
                                        .Filename == filename)
                                {
                                    // optimize by only change volume
                                    element = HitsoundNode.CreateLoopVolumeSignal((int)timing.Offset, volume);
                                }
                                else
                                {
                                    SlideChannel channel;
                                    if (hitsoundType.HasFlag(HitsoundType.Slide))
                                        channel = SlideChannel.Normal;
                                    else if (hitsoundType.HasFlag(HitsoundType.SlideWhistle))
                                        channel = SlideChannel.Whistle;
                                    else
                                        continue;

                                    // new sample
                                    element = HitsoundNode.CreateLoopSignal((int)timing.Offset, volume, balance,
                                        filename, useUserSkin, channel);
                                }

                                slideElements.Add(element);
                            }

                            continue;
                        }

                        // optimize useless timing point
                        timingsOnSlider.RemoveAt(i);
                        i--;
                    }

                    // end slide
                    var stopElement = HitsoundNode.CreateLoopStopSignal((int)endOffset, SlideChannel.Normal);
                    var stopElement2 = HitsoundNode.CreateLoopStopSignal((int)endOffset, SlideChannel.Whistle);
                    slideElements.Add(stopElement);
                    slideElements.Add(stopElement2);
                    foreach (var slideElement in slideElements)
                    {
                        elements.Add(slideElement);
                    }
                }

                // change balance while sliding (not supported in original game)
                var trails = hitObject.SliderInfo.GetSliderSlides();
                var all = trails
                    .Select(k => new
                    {
                        offset = k.Offset,
                        balance = ignoreBalance ? 0 : GetObjectBalance(k.Point.X)
                    })
                    .Select(k => HitsoundNode.CreateLoopBalanceSignal((int)k.offset, k.balance));
                foreach (var balanceElement in all)
                {
                    elements.Add(balanceElement);
                }
            }
        }

        private IEnumerable<(string filename, bool useUserSkin, HitsoundType hitsoundType)> AnalyzeHitsoundFiles(
            HitsoundType itemHitsound,
            ObjectSamplesetType itemSample,
            ObjectSamplesetType itemAddition,
            TimingPoint timingPoint,
            RawHitObject hitObject,
            OsuFile osuFile)
        {
            if (!string.IsNullOrEmpty(hitObject.FileName))
            {
                var filename = _cache.GetFileUntilFind(_directory,
                    Path.GetFileNameWithoutExtension(hitObject.FileName)!,
                    out _);
                return new[] { ValueTuple.Create(filename, false, itemHitsound) };
            }

            var tuples = new List<(string, bool, HitsoundType)>();

            // hitnormal, sliderslide
            string sampleStr = itemSample != ObjectSamplesetType.Auto
                ? itemSample.ToHitsoundString(null)!
                : timingPoint.TimingSampleset.ToHitsoundString();

            // hitclap, hitfinish, hitwhistle, slidertick, sliderwhistle
            string additionStr = itemAddition.ToHitsoundString(sampleStr)!;

            Debug.Assert(sampleStr != null);
            Debug.Assert(additionStr != null);

            if (hitObject.ObjectType == HitObjectType.Slider && hitObject.SliderInfo.EdgeHitsounds == null)
            {
                var hitsounds = GetHitsounds(itemHitsound, sampleStr, additionStr);
                tuples.AddRange(hitsounds);
            }
            else
            {
                var hitsounds = GetHitsounds(itemHitsound, sampleStr, additionStr);
                tuples.AddRange(osuFile.General.Mode == GameMode.Mania
                    ? hitsounds.Take(1)
                    : hitsounds);
            }

            for (var i = 0; i < tuples.Count; i++)
            {
                var fileNameWithoutIndex = tuples[i].Item1;
                var hitsoundType = tuples[i].Item3;

                int baseIndex = hitObject.CustomIndex > 0 ? hitObject.CustomIndex : timingPoint.Track;
                string indexStr = baseIndex > 1 ? baseIndex.ToString() : "";

                var fileNameWithoutExt = fileNameWithoutIndex + indexStr;

                bool useUserSkin;
                string filename;
                if (timingPoint.Track == 0)
                {
                    filename = fileNameWithoutExt + HitsoundFileCache.WavExtension;
                    useUserSkin = true;
                }
                else if (WaveFiles!.Contains(fileNameWithoutExt))
                {
                    filename = _cache.GetFileUntilFind(_directory, fileNameWithoutExt, out useUserSkin);
                }
                else
                {
                    filename = fileNameWithoutIndex + HitsoundFileCache.WavExtension;
                    useUserSkin = true;
                }

                tuples[i] = (filename, useUserSkin, hitsoundType);
            }

            return tuples;
        }

        private static float GetObjectBalance(float x)
        {
            if (x > 512) x = 512;
            else if (x < 0) x = 0;

            float balance = (x - 256f) / 256f;
            return balance;
        }

        private static float GetObjectVolume(RawHitObject obj, TimingPoint timingPoint)
        {
            return (obj.SampleVolume != 0 ? obj.SampleVolume : timingPoint.Volume) / 100f;
        }

        private static IEnumerable<(string, bool, HitsoundType)> GetHitsounds(HitsoundType type,
            string? sampleStr, string? additionStr)
        {
            if (type == HitsoundType.Tick)
            {
                yield return ($"{additionStr}-slidertick", false, type);
                yield break;
            }

            if (type.HasFlag(HitsoundType.Slide))
                yield return ($"{sampleStr}-sliderslide", false, type);
            if (type.HasFlag(HitsoundType.SlideWhistle))
                yield return ($"{additionStr}-sliderwhistle", false, type);

            if (type.HasFlag(HitsoundType.Slide) || type.HasFlag(HitsoundType.SlideWhistle))
                yield break;

            if (type.HasFlag(HitsoundType.Whistle))
                yield return ($"{additionStr}-hitwhistle", false, type);
            if (type.HasFlag(HitsoundType.Clap))
                yield return ($"{additionStr}-hitclap", false, type);
            if (type.HasFlag(HitsoundType.Finish))
                yield return ($"{additionStr}-hitfinish", false, type);
            if (type.HasFlag(HitsoundType.Normal) ||
                (type & HitsoundType.Normal) == 0)
                yield return ($"{sampleStr}-hitnormal", false, type);
        }
    }
}