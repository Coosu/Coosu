namespace Coosu.Storyboard.Storybrew.Text;

public class FontFamilySource
{
    public string Name { get; }
    public string? Path { get; private set; }

    public FontFamilySource(string name)
    {
        Name = name;
    }

    public static FontFamilySource FromFiles(string path, string name)
    {
        return new FontFamilySource(name) { Path = path };
    }

    public static implicit operator FontFamilySource(string s)
    {
        return new FontFamilySource(s);
    }
}