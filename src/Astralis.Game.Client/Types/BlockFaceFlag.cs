namespace Astralis.Game.Client.Types;

[Flags]
public enum BlockFaceFlag
{
    Empty = 0,
    Top = 1,
    Bottom = 2,
    Left = 4,
    Right = 8,
    Front = 16,
    Back = 32
}
