using System;
using System.IO;
using System.Text;
using Coosu.Beatmap;
using Xunit;

namespace CoosuUnitTest.Beatmap;

public class EventSectionTest
{
    [Theory]
    [InlineData("//Break Periods")]
    [InlineData("//Background and Video events")]
    [InlineData("//Storyboard Layer 0 (Background)")]
    [InlineData("//Custom events")]
    [InlineData("")]
    public void EventTypes_AreIndependentOfCommentHeadings(string heading)
    {
        var file = ReadEvents(heading + "\n" + """
            0,0,"bg.jpg",10,20
            2,100,200
            Sprite,Background,Centre,"bg.jpg",320,240
             F,0,0,1000,1
            Animation,Foreground,Centre,"anim.png",320,240,4,100,LoopForever
            Sample,500,0,"sample.wav",80
            5,750,0,"sample2.wav",50
            Video,100,"video.mp4"
            """);

        var events = file.Events!;
        Assert.Equal("bg.jpg", events.BackgroundInfo!.Filename);
        Assert.Equal(10, events.BackgroundInfo.X);
        Assert.Equal(20, events.BackgroundInfo.Y);
        var pause = Assert.Single(events.Breaks);
        Assert.Equal(100, pause.StartTime);
        Assert.Equal(200, pause.EndTime);
        Assert.Equal("video.mp4", events.VideoInfo!.Filename);
        Assert.Equal(100, events.VideoInfo.Offset);
        Assert.Collection(events.Samples,
            sample => { Assert.Equal(500, sample.Offset); Assert.Equal("sample.wav", sample.Filename); Assert.Equal(80, sample.Volume); },
            sample => { Assert.Equal(750, sample.Offset); Assert.Equal("sample2.wav", sample.Filename); Assert.Equal(50, sample.Volume); });
        Assert.Contains("Sprite,Background,Centre", events.StoryboardText);
        Assert.Contains(" F,0,0,1000,1", events.StoryboardText);
        Assert.Contains("Animation,Foreground,Centre", events.StoryboardText);
        Assert.DoesNotContain("Sample,500", events.StoryboardText);
    }

    [Fact]
    public void Storyboard_IsRetainedWithoutASampleHeadingAtTheEnd()
    {
        var file = ReadEvents("""
            //Break Periods
            2,100,200
            Sprite,Background,Centre,"bg.jpg",320,240
             F,0,0,1000,1
            """);

        Assert.Contains("Sprite,Background,Centre", file.Events!.StoryboardText);
        Assert.EndsWith(" F,0,0,1000,1", file.Events.StoryboardText);
    }

    [Fact]
    public void Storyboard_IsRetainedAcrossRepeatedSampleHeadingsAndRoundTrip()
    {
        var file = ReadEvents("""
            //Storyboard Layer 0 (Background)
            Sprite,Background,Centre,"first.jpg",320,240
             F,0,0,1000,1
            //Storyboard Sound Samples
            Sample,500,0,"sample.wav",80
            Sprite,Foreground,Centre,"second.jpg",320,240
             F,0,1000,2000,1,0
            //Storyboard Sound Samples
            5,1500,0,"sample2.wav",50
            """);
        using var writer = new StringWriter();
        file.SaveTo(writer);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(writer.ToString()));
        var roundTrip = OsuFile.ReadFromStream(stream);

        Assert.Equal(file.Events!.StoryboardText, roundTrip.Events!.StoryboardText);
        Assert.Contains("first.jpg", roundTrip.Events.StoryboardText);
        Assert.Contains("second.jpg", roundTrip.Events.StoryboardText);
        Assert.Equal(2, roundTrip.Events.Samples.Count);
        Assert.Equal(1500, roundTrip.Events.Samples[1].Offset);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void IgnoreOptions_AffectOnlyTheirOwnEventTypes(bool ignoreStoryboard, bool ignoreSamples)
    {
        var file = ReadEvents("""
            //Break Periods
            2,100,200
            Sprite,Background,Centre,"bg.jpg",320,240
             F,0,0,1000,1
            Sample,500,0,"sample.wav",80
            """, options =>
        {
            if (ignoreStoryboard) options.IgnoreStoryboard();
            if (ignoreSamples) options.IgnoreSample();
        });

        Assert.Single(file.Events!.Breaks);
        if (ignoreStoryboard) Assert.Null(file.Events.StoryboardText);
        else Assert.Contains("Sprite,Background,Centre", file.Events.StoryboardText);
        if (ignoreSamples) Assert.Empty(file.Events.Samples);
        else Assert.Single(file.Events.Samples);
    }

    [Fact]
    public void RepeatedUnknownComments_DoNotPreventParsingKnownEvents()
    {
        var file = ReadEvents("""
            //custom
            extra,first
            //custom
            extra,second
            Sample,500,0,"sample.wav",80
            """);
        using var writer = new StringWriter();
        file.SaveTo(writer);

        Assert.Single(file.Events!.Samples);
        Assert.Contains("extra,first", writer.ToString());
        Assert.Contains("extra,second", writer.ToString());
    }

    [Fact]
    public void StoryboardText_CanBeReadReplacedAndExtended()
    {
        var events = ReadEvents("Sprite,Background,Centre,\"first.jpg\",320,240").Events!;
        Assert.Contains("first.jpg", events.StoryboardText);
        events.Match(" F,0,0,1000,1".AsMemory());
        Assert.Contains(" F,0,0,1000,1", events.StoryboardText);

        events.StoryboardText = "Sprite,Background,Centre,\"replacement.jpg\",320,240";
        events.Match(" F,0,1000,2000,0".AsMemory());

        Assert.Contains("replacement.jpg", events.StoryboardText);
        Assert.Contains(" F,0,1000,2000,0", events.StoryboardText);
        Assert.DoesNotContain("first.jpg", events.StoryboardText);
    }

    [Fact]
    public void NumericVisualEvents_ArePreservedWithoutHeadings()
    {
        var file = ReadEvents("""
            4,Background,Centre,"bg.jpg",320,240
             F,0,0,1000,1
            6,Foreground,Centre,"anim.png",320,240,4,100,LoopForever
            """);

        Assert.Contains("4,Background,Centre", file.Events!.StoryboardText);
        Assert.Contains("6,Foreground,Centre", file.Events.StoryboardText);
    }

    [Theory]
    [InlineData("Sample,invalid,0,\"sample.wav\",80")]
    [InlineData("2,invalid,200")]
    public void InvalidKnownEvents_StillReportParsingErrors(string line)
    {
        Assert.Throws<FormatException>(() => ReadEvents(line));
    }

    private static OsuFile ReadEvents(string events, Action<OsuReadOptions>? configure = null)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("osu file format v14\n[Events]\n" + events));
        return OsuFile.ReadFromStream(stream, configure);
    }
}
