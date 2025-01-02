using Astralis.Game.Client.Types;
using Silk.NET.Maths;

namespace Astralis.Game.Client.Utils;

public static class FaceOffset
{
    public static Vector3D<int> GetOffsetOfFace(BlockFace blockFace)
    {
        return blockFace switch
        {
            BlockFace.TOP    => new Vector3D<int>(0, 1, 0),
            BlockFace.BOTTOM => new Vector3D<int>(0, -1, 0),
            BlockFace.LEFT   => new Vector3D<int>(-1, 0, 0),
            BlockFace.RIGHT  => new Vector3D<int>(1, 0, 0),
            BlockFace.FRONT  => new Vector3D<int>(0, 0, 1),
            BlockFace.BACK   => new Vector3D<int>(0, 0, -1),
            _                => Vector3D<int>.Zero
        };
    }
}
