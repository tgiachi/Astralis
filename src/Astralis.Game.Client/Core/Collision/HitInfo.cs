namespace Astralis.Game.Client.Core.Collision;

public struct HitInfo
{
    public bool IsHit { get; }
    public float FNorm { get; }

    public HitInfo(bool isHit, float fNorm)
    {
        IsHit = isHit;
        FNorm = fNorm;
    }
}
