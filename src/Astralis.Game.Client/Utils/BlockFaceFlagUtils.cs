using Astralis.Game.Client.Types;

namespace Astralis.Game.Client.Utils;

public class BlockFaceFlagUtils
{
    public static readonly BlockFace[] FACES =
        [BlockFace.TOP, BlockFace.BOTTOM, BlockFace.LEFT, BlockFace.RIGHT, BlockFace.FRONT, BlockFace.BACK];

    public static int NbFaces(BlockFaceFlag facesFlag)
    {
        int total = 0;
        if ((facesFlag & BlockFaceFlag.Top) == BlockFaceFlag.Top)
        {
            total++;
        }

        if ((facesFlag & BlockFaceFlag.Bottom) == BlockFaceFlag.Bottom)
        {
            total++;
        }

        if ((facesFlag & BlockFaceFlag.Left) == BlockFaceFlag.Left)
        {
            total++;
        }

        if ((facesFlag & BlockFaceFlag.Right) == BlockFaceFlag.Right)
        {
            total++;
        }

        if ((facesFlag & BlockFaceFlag.Front) == BlockFaceFlag.Front)
        {
            total++;
        }

        if ((facesFlag & BlockFaceFlag.Back) == BlockFaceFlag.Back)
        {
            total++;
        }

        return total;
    }

    public static IEnumerable<BlockFace> GetFaces(BlockFaceFlag facesFlag)
    {
        if ((facesFlag & BlockFaceFlag.Top) == BlockFaceFlag.Top)
        {
            yield return BlockFace.TOP;
        }

        if ((facesFlag & BlockFaceFlag.Bottom) == BlockFaceFlag.Bottom)
        {
            yield return BlockFace.BOTTOM;
        }

        if ((facesFlag & BlockFaceFlag.Left) == BlockFaceFlag.Left)
        {
            yield return BlockFace.LEFT;
        }

        if ((facesFlag & BlockFaceFlag.Right) == BlockFaceFlag.Right)
        {
            yield return BlockFace.RIGHT;
        }

        if ((facesFlag & BlockFaceFlag.Front) == BlockFaceFlag.Front)
        {
            yield return BlockFace.FRONT;
        }

        if ((facesFlag & BlockFaceFlag.Back) == BlockFaceFlag.Back)
        {
            yield return BlockFace.BACK;
        }
    }

    public static BlockFaceExtended? GetFaceExtended(BlockFaceFlag BlockFaceFlag)
    {
        return BlockFaceFlag switch
        {
            BlockFaceFlag.Empty => null,
            BlockFaceFlag.Top => BlockFaceExtended.Top,
            BlockFaceFlag.Bottom => BlockFaceExtended.Bottom,
            BlockFaceFlag.Back => BlockFaceExtended.Back,
            BlockFaceFlag.Front => BlockFaceExtended.Front,
            BlockFaceFlag.Left => BlockFaceExtended.Left,
            BlockFaceFlag.Right => BlockFaceExtended.Right,
            BlockFaceFlag.Top | BlockFaceFlag.Left => BlockFaceExtended.LeftTop,
            BlockFaceFlag.Top | BlockFaceFlag.Right => BlockFaceExtended.RightTop,
            BlockFaceFlag.Top | BlockFaceFlag.Front => BlockFaceExtended.TopFront,
            BlockFaceFlag.Top | BlockFaceFlag.Back => BlockFaceExtended.TopBack,
            BlockFaceFlag.Bottom | BlockFaceFlag.Left => BlockFaceExtended.LeftBottom,
            BlockFaceFlag.Bottom | BlockFaceFlag.Right => BlockFaceExtended.RightBottom,
            BlockFaceFlag.Bottom | BlockFaceFlag.Front => BlockFaceExtended.BottomFront,
            BlockFaceFlag.Bottom | BlockFaceFlag.Back => BlockFaceExtended.BottomBack,
            BlockFaceFlag.Left | BlockFaceFlag.Top | BlockFaceFlag.Front => BlockFaceExtended.LeftTopFront,
            BlockFaceFlag.Right | BlockFaceFlag.Top | BlockFaceFlag.Front => BlockFaceExtended.RightTopFront,
            BlockFaceFlag.Left | BlockFaceFlag.Top | BlockFaceFlag.Back => BlockFaceExtended.LeftTopBack,
            BlockFaceFlag.Right | BlockFaceFlag.Top | BlockFaceFlag.Back => BlockFaceExtended.RightTopBack,
            BlockFaceFlag.Left | BlockFaceFlag.Bottom | BlockFaceFlag.Front => BlockFaceExtended.LeftBottomFront,
            BlockFaceFlag.Right | BlockFaceFlag.Bottom | BlockFaceFlag.Front => BlockFaceExtended.RightBottomFront,
            BlockFaceFlag.Left | BlockFaceFlag.Bottom | BlockFaceFlag.Back => BlockFaceExtended.LeftBottomBack,
            BlockFaceFlag.Right | BlockFaceFlag.Bottom | BlockFaceFlag.Back => BlockFaceExtended.RightBottomBack,
            BlockFaceFlag.Left | BlockFaceFlag.Front => BlockFaceExtended.LeftFront,
            BlockFaceFlag.Right | BlockFaceFlag.Front => BlockFaceExtended.RightFront,
            BlockFaceFlag.Left | BlockFaceFlag.Back => BlockFaceExtended.LeftBack,
            BlockFaceFlag.Right | BlockFaceFlag.Back => BlockFaceExtended.RightBack,
            _ => throw new Exception("BlockFaceFlag not found")
        };
    }
}
