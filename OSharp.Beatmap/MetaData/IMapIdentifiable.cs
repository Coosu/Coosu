namespace OSharp.Beatmap.MetaData
{
    public interface IMapIdentifiable
    {
        string Version { get; }
        string FolderName { get; }
        bool InOwnDb { get; }

        MapIdentity GetIdentity();
    }
}
