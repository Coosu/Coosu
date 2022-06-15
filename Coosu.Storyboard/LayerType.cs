namespace Coosu.Storyboard
{
    /// <summary>
    /// osu!storyboard layer which the object appears on.
    /// <para><seealso href="https://osu.ppy.sh/wiki/Storyboard_Scripting/Objects"/></para>
    /// </summary>
    public enum LayerType : byte
    {
        Background,
        Fail,
        Pass,
        Foreground,
        Overlay
    }
}
