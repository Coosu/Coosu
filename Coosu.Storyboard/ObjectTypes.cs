namespace Coosu.Storyboard;

/// <summary>
/// Built-in osu!storyboard object definition.
/// <para><seealso href="https://osu.ppy.sh/wiki/Storyboard_Scripting/Objects"/></para>
/// </summary>
public static class ObjectTypes
{
    public static ObjectType Background { get; } = new(0);
    public static ObjectType Video { get; } = new(1);
    public static ObjectType Break { get; } = new(2);
    public static ObjectType Color { get; } = new(3);

    /// <summary>
    /// Basic image.
    /// </summary>
    public static ObjectType Sprite { get; } = new(4);

    public static ObjectType Sample { get; } = new(5);

    /// <summary>
    /// Moving image.
    /// </summary>
    public static ObjectType Animation { get; } = new(6);
}