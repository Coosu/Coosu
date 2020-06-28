using System;

namespace OSharp.Api.V1
{
    /// <summary>
    /// Mod of a game play.
    /// </summary>
    [Flags]
    public enum Mod
    {
        /// <summary>
        /// None mod.
        /// </summary>
        None =        0x00000000,
        /// <summary>
        /// NoFail mod.
        /// </summary>
        NoFail =      0x00000001,
        /// <summary>
        /// Easy mod.
        /// </summary>
        Easy =        0x00000002,
        /// <summary>
        /// TouchDevice mod. (Hidden in osu! client)
        /// </summary>
        TouchDevice = 0x00000004,
        /// <summary>
        /// Hidden mod.
        /// </summary>
        Hidden =      0x00000008,
        /// <summary>
        /// HardRock mod.
        /// </summary>
        HardRock =    0x00000010,
        /// <summary>
        /// SuddenDeath mod.
        /// </summary>
        SuddenDeath = 0x00000020,
        /// <summary>
        /// DoubleTime mod.
        /// </summary>
        DoubleTime =  0x00000040,
        /// <summary>
        /// Relax mod.
        /// </summary>
        Relax =       0x00000080,
        /// <summary>
        /// HalfTime mod.
        /// </summary>
        HalfTime =    0x00000100,
        /// <summary>
        /// NightCore mod.
        /// Only set along with DoubleTime. i.e: NC only gives 576
        /// </summary>
        NightCore =   0x00000200,
        /// <summary>
        /// Flashlight mod.
        /// </summary>
        Flashlight =  0x00000400,
        /// <summary>
        /// Autoplay mod.
        /// </summary>
        Autoplay =    0x00000800,
        /// <summary>
        /// SpunOut mod.
        /// </summary>
        SpunOut =     0x00001000,
        /// <summary>
        /// Autopilot mod.
        /// </summary>
        Relax2 =      0x00002000,
        /// <summary>
        /// Perfect mod.
        /// Only set along with SuddenDeath. i.e: PF only gives 16416  
        /// </summary>
        Perfect =     0x00004000,
        /// <summary>
        /// 4K mod.
        /// </summary>
        Key4 =        0x00008000,
        /// <summary>
        /// 5K mod.
        /// </summary>
        Key5 =        0x00010000,
        /// <summary>
        /// 6K mod.
        /// </summary>
        Key6 =        0x00020000,
        /// <summary>
        /// 7K mod.
        /// </summary>
        Key7 =        0x00040000,
        /// <summary>
        /// 8K mod.
        /// </summary>
        Key8 =        0x00080000,
        /// <summary>
        /// FadeIn mod.
        /// </summary>
        FadeIn =      0x00100000,
        /// <summary>
        /// Random mod.
        /// </summary>
        Random =      0x00200000,
        /// <summary>
        /// Cinema mod.
        /// </summary>
        Cinema =      0x00400000,
        /// <summary>
        /// Target mod.
        /// </summary>
        Target =      0x00800000,
        /// <summary>
        /// 9K mod.
        /// </summary>
        Key9 =        0x01000000,
        /// <summary>
        /// KeyCoop mod.
        /// </summary>
        KeyCoop =     0x02000000,
        /// <summary>
        /// 1K mod.
        /// </summary>
        Key1 =        0x04000000,
        /// <summary>
        /// 3K mod.
        /// </summary>
        Key3 =        0x08000000,
        /// <summary>
        /// 2K mod.
        /// </summary>
        Key2 =        0x10000000,
        /// <summary>
        /// ScoreV2 mod.
        /// </summary>
        ScoreV2 =     0x20000000,
        /// <summary />
        LastMod =     0x40000000,
        /// <summary>
        /// Use key mod.
        /// </summary>
        KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,
        /// <summary>
        /// Represents whether free mod is allowed.
        /// </summary>
        FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
        /// <summary>
        /// Represents whether the mod is score increasing mods.
        /// </summary>
        ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
    }
}
