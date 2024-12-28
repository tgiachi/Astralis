namespace Astralis.Game.Client.Core.Collision;

public interface Volume
{

    public bool IsInFrustrum(Frustum frustum);
}
