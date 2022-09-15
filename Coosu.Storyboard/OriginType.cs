namespace Coosu.Storyboard;

/// <summary>
/// Definition of where on the image should osu! consider that image's origin (coordinate) to be.
/// <para><seealso href="https://osu.ppy.sh/wiki/Storyboard_Scripting/Objects"/></para>
/// </summary>
public enum OriginType
{
    TopLeft = 0,
    TopCentre = 5,
    TopRight = 3,
    CentreLeft = 2,
    Centre = 1,
    CentreRight = 7,
    BottomLeft = 8,
    BottomCentre = 4,
    BottomRight = 9,

    /// <summary>
    /// Same effect as TopLeft, but should not be used.
    /// </summary>
    Custom = 6,
}