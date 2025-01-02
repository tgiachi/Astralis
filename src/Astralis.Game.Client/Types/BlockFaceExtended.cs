using System.Collections.ObjectModel;
using Silk.NET.Maths;

namespace Astralis.Game.Client.Types;

public enum BlockFaceExtended
{
    Top = 0,
    Bottom = 1,
    Left = 2,
    Right = 3,
    Front = 4,
    Back = 5,

    LeftTop = 6,
    RightTop = 7,
    TopFront = 8,
    TopBack = 9,

    LeftBottom = 10,
    RightBottom = 11,
    BottomFront = 12,
    BottomBack = 13,

    LeftTopFront = 14,
    RightTopFront = 15,
    LeftTopBack = 16,
    RightTopBack = 17,

    LeftBottomFront = 18,
    RightBottomFront = 19,
    LeftBottomBack = 20,
    RightBottomBack = 21,

    LeftFront = 22,
    RightFront = 23,
    LeftBack = 24,
    RightBack = 25
};

public static class FaceExtendedConst
{
    public static readonly ReadOnlyCollection<BlockFaceExtended> FACES = new(Enum.GetValues<BlockFaceExtended>());
}

public static class FaceExtendedOffset
{
    public static Vector3D<int> GetOffsetOfFace(BlockFaceExtended face)
    {
        return _faceOffsets[(int)face];
    }


    private static readonly Vector3D<int>[] _faceOffsets = new Vector3D<int>[]
    {
        new Vector3D<int>(0, 1, 0),
        new Vector3D<int>(0, -1, 0),
        new Vector3D<int>(-1, 0, 0),
        new Vector3D<int>(1, 0, 0),
        new Vector3D<int>(0, 0, 1),
        new Vector3D<int>(0, 0, -1),
        new Vector3D<int>(-1, 1, 0),
        new Vector3D<int>(1, 1, 0),
        new Vector3D<int>(0, 1, 1),
        new Vector3D<int>(0, 1, -1),
        new Vector3D<int>(-1, -1, 0),
        new Vector3D<int>(1, -1, 0),
        new Vector3D<int>(0, -1, 1),
        new Vector3D<int>(0, -1, -1),
        new Vector3D<int>(-1, 1, 1),
        new Vector3D<int>(1, 1, 1),
        new Vector3D<int>(-1, 1, -1),
        new Vector3D<int>(1, 1, -1),
        new Vector3D<int>(-1, -1, 1),
        new Vector3D<int>(1, -1, 1),
        new Vector3D<int>(-1, -1, -1),
        new Vector3D<int>(1, -1, -1),
        new Vector3D<int>(-1, 0, 1),
        new Vector3D<int>(1, 0, 1),
        new Vector3D<int>(-1, 0, -1),
        new Vector3D<int>(1, 0, -1),
    };
}
