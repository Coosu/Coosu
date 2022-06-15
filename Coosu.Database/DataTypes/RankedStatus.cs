namespace Coosu.Database.DataTypes;

public enum RankedStatus : byte
{
    Unknown = 0,
    Unsubmitted = 1,
    Pending = 2,
    Unused = 3,
    Ranked = 4,
    Approved = 5,
    Qualified = 6,
    Loved = 7
}