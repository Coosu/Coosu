using System;

namespace Coosu.Database.DataTypes;

[Flags]
public enum Permissions
{
    None = 0,
    Normal = 1,
    Moderator = 2,
    Supporter = 4,
    Friend = 8,
    Peppy = 16,
    WorldCupStaff = 32
}