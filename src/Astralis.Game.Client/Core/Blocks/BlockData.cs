using Astralis.Core.World.Blocks;

namespace Astralis.Game.Client.Core.Blocks;

public struct BlockData
{
    public const int SIZEOF_SERIALIZE_DATA = 4;

    // 16 bits for block id, 4 bits for light level, 4 bits for skylight level

    public int Data { get; private set; }

    public readonly int Id => Data & 0xFFFF;


    public BlockData(ReadOnlySpan<byte> buffer)
    {
        Data = BitConverter.ToInt32(buffer);
    }

    public BlockData(BlockType id)
    {
        Data = (int)id & 0xFFFF;
    }

    public BlockData(BinaryReader br)
    {
        Data = br.ReadInt32();
    }

    public byte GetLightLevel()
    {
        return (byte)((Data >> 16) & 0xF);
    }

    public void SetLightLevel(byte lightLevel)
    {
        Data = (Data & ~(0xF << 16)) | (lightLevel << 16);
    }

    public byte GetSkyLightLevel()
    {
        return (byte)(((Data >> 16) & 0xF0) >> 4);
    }

    public void SetSkyLightLevel(byte lightLevel)
    {
        Data = (Data & ~(0xF0 << 16)) | (lightLevel << (4 + 16));
    }

    public void WriteToStream(BinaryWriter bw)
    {
        bw.Write(Data);
    }

    public override bool Equals(object? obj)
    {
        return obj is BlockData block && Data == block.Data;
    }

    public bool Equals(BlockData other)
    {
        return Data == other.Data;
    }

    public override int GetHashCode()
    {
        return Data & 0xFFFF;
    }
}
