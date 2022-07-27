using System.Diagnostics;

namespace Coosu.Beatmap.MetaData;

[DebuggerDisplay("{DebuggerDisplay()}")]
public readonly struct MapIdentity : IMapIdentifiable
{
    private static readonly MapIdentity _default = new();

    public MapIdentity(string folderName, string version, bool inOwnDb) : this()
    {
        FolderName = folderName;
        Version = version;
        InOwnDb = inOwnDb;
    }

    public string FolderName { get; }
    public string Version { get; }
    public bool InOwnDb { get; }
    public MapIdentity GetIdentity() => this;

    public static ref readonly MapIdentity Default => ref _default;

    public override bool Equals(object obj)
    {
        if (obj is not MapIdentity mi)
        {
            return false;
        }

        return mi.FolderName == FolderName && mi.Version == Version;
    }

    public override int GetHashCode() => base.GetHashCode();

    private string DebuggerDisplay()
    {
        if (this.IsMapTemporary())
            return $"temp: \"{FolderName}\"";

        if (InOwnDb)
            return $"own: [\"{FolderName}\",\"{Version}\"]";

        return $"osu: [\"{FolderName}\",\"{Version}\"]";
    }
}