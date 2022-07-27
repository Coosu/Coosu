namespace Coosu.Beatmap;

public enum OverlayPosition : byte
{
    /// <summary>
    /// use skin setting
    /// </summary>
    NoChange,

    /// <summary>
    /// draw overlays under numbers
    /// </summary>
    Below,

    /// <summary>
    /// draw overlays on top of numbers
    /// </summary>
    Above
}