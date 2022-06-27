using System.Collections.Generic;

namespace Coosu.Beatmap.Configurable;

public class FlagRule
{
    public string SplitFlag { get; }
    public TrimType TrimType { get; }

    public FlagRule(string splitFlag, TrimType trimType)
    {
        SplitFlag = splitFlag;
        TrimType = trimType;
    }
}

public static class FlagRules
{
    public static FlagRule Colon { get; } = new FlagRule(":", TrimType.None);
    public static FlagRule ColonSpace { get; } = new FlagRule(": ", TrimType.None);
    public static FlagRule SpaceColonSpace { get; } = new FlagRule(" : ", TrimType.None);

    public static IReadOnlyList<FlagRule> FuzzyRules { get; } = new FlagRule[]
    {
        new FlagRule(":", TrimType.Both)
    };
}