using System.Diagnostics;

namespace Coosu.Beatmap.MetaData;

[DebuggerDisplay("{DebuggerDisplay()}")]
public readonly struct MapIdentity : IMapIdentifiable
{
    private static readonly MapIdentity BackFieldDefault = new();

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

    public static ref readonly MapIdentity Default => ref BackFieldDefault;

    public override bool Equals(object? obj)
    {
        if (obj is not MapIdentity mi)
        {
            return false;
        }

        return Equals(mi);
    }

    public bool Equals(MapIdentity other)
    {
        return FolderName == other.FolderName && Version == other.Version && InOwnDb == other.InOwnDb;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = FolderName.GetHashCode();
            hashCode = (hashCode * 397) ^ Version.GetHashCode();
            hashCode = (hashCode * 397) ^ InOwnDb.GetHashCode();
            return hashCode;
        }
    }

    public static bool operator ==(MapIdentity left, MapIdentity right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(MapIdentity left, MapIdentity right)
    {
        return !left.Equals(right);
    }


    private string DebuggerDisplay()
    {
        if (this.IsMapTemporary())
            return $"temp: \"{FolderName}\"";

        if (InOwnDb)
            return $"own: [\"{FolderName}\",\"{Version}\"]";

        return $"osu: [\"{FolderName}\",\"{Version}\"]";
    }
}