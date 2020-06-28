using System;

namespace OSharp.Storyboard.Events.Containers
{
    [Flags]
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
}
