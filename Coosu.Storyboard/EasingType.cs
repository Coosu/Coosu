namespace Coosu.Storyboard;

/// <summary>
/// osu!storyboard built-in easing types
/// <para><seealso href="https://osu.ppy.sh/wiki/Storyboard_Scripting/Commands"/></para>
/// </summary>
public enum EasingType
{
    /// <summary>
    /// No easing.
    /// </summary>
    Linear,

    /// <summary>
    /// The changes happen fast at first, but then slow down toward the end.
    /// </summary>
    EasingOut,

    /// <summary>
    /// The changes happen slowly at first, but then speed up toward the end.
    /// </summary>
    EasingIn,
    QuadIn, QuadOut, QuadInOut,
    CubicIn, CubicOut, CubicInOut,
    QuartIn, QuartOut, QuartInOut,
    QuintIn, QuintOut, QuintInOut,
    SineIn, SineOut, SineInOut,
    ExpoIn, ExpoOut, ExpoInOut,
    CircIn, CircOut, CircInOut,
    ElasticIn, ElasticOut, ElasticHalfOut, ElasticQuarterOut, ElasticInOut,
    BackIn, BackOut, BackInOut,
    BounceIn, BounceOut, BounceInOut
}