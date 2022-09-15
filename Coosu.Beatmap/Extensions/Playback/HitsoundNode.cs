using System;

namespace Coosu.Beatmap.Extensions.Playback;

public abstract class HitsoundNode
{
    /// <summary>
    /// 0 - 1.0f
    /// </summary>
    public float Balance { get; internal set; }
    public string? Filename { get; internal set; }
    public int Offset { get; internal set; }
    public bool UseUserSkin { get; internal set; }

    /// <summary>
    /// 0 - 1.0f
    /// </summary>
    public float Volume { get; internal set; }

    public static PlayableNode Create(
        Guid guid,
        int offset,
        float volume,
        float balance,
        string filename,
        bool useUserSkin,
        PlayablePriority playablePriority)
    {
        var soundElement = new PlayableNode
        {
            Guid = guid,
            Offset = offset,
            Volume = volume,
            Balance = balance,
            Filename = filename,
            UseUserSkin = useUserSkin,
            PlayablePriority = playablePriority
        };
        return soundElement;
    }

    public static ControlNode CreateLoopSignal(
        int offset,
        float volume,
        float balance,
        string filename,
        bool useUserSkin,
        SlideChannel loopChannel)
    {
        return new ControlNode
        {
            Offset = offset,
            Volume = volume,
            Balance = balance,
            Filename = filename,
            UseUserSkin = useUserSkin,

            ControlType = ControlType.StartSliding,
            SlideChannel = loopChannel
        };
    }

    public static ControlNode CreateLoopStopSignal(int offset, SlideChannel loopChannel)
    {
        return new ControlNode
        {
            Offset = offset,

            ControlType = ControlType.StopSliding,
            SlideChannel = loopChannel
        };
    }

    public static ControlNode CreateLoopVolumeSignal(int offset, float volume)
    {
        return new ControlNode
        {
            Offset = offset,
            Volume = volume,

            ControlType = ControlType.ChangeVolume
        };
    }

    public static ControlNode CreateLoopBalanceSignal(int offset, float balance)
    {
        return new ControlNode
        {
            Offset = offset,
            Balance = balance,

            ControlType = ControlType.ChangeBalance
        };
    }
}