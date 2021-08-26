using System;

namespace Coosu.Storyboard.Events
{
    [Flags]
#pragma warning disable format
// @formatter:off
    public enum TriggerType
    {
        HitSound         = 0b0000,
        HitSoundWhistle  = 0b0001,
        HitSoundClap     = 0b0010,
        HitSoundFinish   = 0b0011,
        HitSoundSoft     = 0b0100,
        HitSoundNormal   = 0b1000,
        HitSoundDrum     = 0b1100,
        HitSoundAddition = HitSoundWhistle | HitSoundClap | HitSoundFinish,
        HitSoundSample   = HitSoundSoft | HitSoundNormal | HitSoundDrum
    }
// @formatter:on
#pragma warning restore format
}
