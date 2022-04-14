namespace Coosu.Database;

public interface IDbStructure
{
    public int NodeId { get; }
    public string Name { get; }
    public string Path { get; }
    IDbStructure? BaseStructure { get; }
}